using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    internal class ElementPart<TParent, TPart> : PartBase<TParent, TPart>
    {
        public ElementPart(XName xmlName, bool isRequired, Func<TParent, Maybe<TPart>> getter, IValueConverter converter)
            : base(xmlName, isRequired, getter, converter) { }

        public override object Parse(XElement parentNode, ImmutableList<IValueConverter> converters)
        {
            parentNode.CheckNull("parentNode");

            XElement element;
            if (!TryGetValidatedElement(parentNode, out element))
                return null;

            bool supplyElement;
            IValueConverter selectedConverter;
            SelectConverter(element, converters, out selectedConverter, out supplyElement);

            return Parse(element, selectedConverter, supplyElement);
        }

        public override XObject Serialize(TParent parent, ImmutableList<IValueConverter> converters)
        {
            HasGetter.Check(v => v,
                            v => new InvalidOperationException("Part with XML name '" + XmlName + "' does not have a getter."));

            TPart value;
            if (!TryGetValidatedValue(parent, out value))
                return null;

            CanConvertResult selectedConverterCanConvertResult;
            IValueConverter selectedConverter;
            SelectConverter(value, converters, out selectedConverter, out selectedConverterCanConvertResult);

            return Serialize(value, selectedConverterCanConvertResult, selectedConverter);
        }

        private TPart Parse(XElement element, IValueConverter converter, bool supplyElement)
        {
            if (supplyElement)
            {
                return (TPart)converter.Convert(element, PartType);
            }

            return (TPart)converter.Convert(element.Value, PartType);
        }

        private void SelectConverter(XElement element, IEnumerable<IValueConverter> converters, out IValueConverter selectedConverter, out bool supplyElement)
        {
            if (Converter != null)
            {
                var canConvertResult = Converter.CanConvert(element, PartType);
                if (canConvertResult != CanConvertResult.ToObject)
                {
                    canConvertResult = Converter.CanConvert(element.Value, PartType);
                    if (canConvertResult != CanConvertResult.ToObject)
                        throw new InvalidOperationException("The specific converter for this part cannot convert the specified value.");

                    supplyElement = false;
                }
                else
                    supplyElement = true;

                selectedConverter = Converter;
                return;
            }

            foreach (var converter in converters)
            {
                var canConvertResult = converter.CanConvert(element, PartType);
                if (canConvertResult != CanConvertResult.ToObject)
                {
                    canConvertResult = converter.CanConvert(element.Value, PartType);
                    if (canConvertResult != CanConvertResult.ToObject)
                        continue;

                    supplyElement = false;
                }
                else
                    supplyElement = true;

                selectedConverter = converter;
                return;
            }

            throw new InvalidOperationException("Unable to serialize: Cannot find compatible converter. Value type: " + typeof(XElement).FullName + ", target type: " + PartType.FullName + ".");
        }

        private void SelectConverter(TPart value, IEnumerable<IValueConverter> converters, out IValueConverter selectedConverter, out CanConvertResult selectedConverterCanConvertResult)
        {
            if (Converter != null)
            {
                var canConvertResult = Converter.CanConvert(value, typeof(XElement));
                if (canConvertResult == CanConvertResult.CannotConvert)
                    throw new InvalidOperationException("The specific converter for this part cannot convert the specified value.");

                selectedConverterCanConvertResult = canConvertResult;
                selectedConverter = Converter;
                return;
            }

            foreach (var converter in converters)
            {
                var canConvertResult = converter.CanConvert(value, typeof(XElement));
                if (canConvertResult == CanConvertResult.CannotConvert)
                    continue;

                selectedConverterCanConvertResult = canConvertResult;
                selectedConverter = converter;
                return;
            }

            throw new InvalidOperationException("Unable to serialize: Cannot find compatible converter. Value type: " + value.GetType().FullName + ", target type: " + typeof(XElement).FullName + ".");
        }

        private XElement Serialize(TPart value, CanConvertResult canConvertResult, IValueConverter converter)
        {
            switch (canConvertResult)
            {
                case CanConvertResult.ToXElement:
                    {
                        var elem = (XElement)converter.Convert(value, typeof(XElement));
                        elem.Name = XmlName;
                        return elem;
                    }
                case CanConvertResult.ToXAttribute:
                    {
                        var attr = (XAttribute)converter.Convert(value, typeof(XElement));
                        var elem = new XElement(XmlName, attr);
                        return elem;
                    }
                case CanConvertResult.ToXmlValue:
                    {
                        var xmlValue = converter.Convert(value, typeof(XElement));
                        var elem = new XElement(XmlName, xmlValue);
                        return elem;
                    }
                default:
                    throw new ArgumentOutOfRangeException("canConvertResult");
            }
        }

        private bool TryGetValidatedElement(XElement parentNode, out XElement element)
        {
            element = parentNode.Element(XmlName);
            if (element == null && IsRequired)
                throw new InvalidOperationException("Cannot parse part; the element is missing and the part is required.");

            return true;
        }

        private bool TryGetValidatedValue(TParent parent, out TPart value)
        {
            var maybeValue = GetValue(parent);
            if (maybeValue.IsMissing && IsRequired)
                throw new InvalidOperationException("Cannot serialize part; the value is missing and the part is required.");

            value = maybeValue.Value;
            return maybeValue.HasValue;
        }
    }
}