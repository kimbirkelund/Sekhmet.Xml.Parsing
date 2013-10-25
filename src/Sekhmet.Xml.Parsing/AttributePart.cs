using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    internal class AttributePart<TParent, TPart> : PartBase<TParent, TPart>
    {
        public AttributePart(XName xmlName, bool isRequired, Func<TParent, Maybe<TPart>> getter, IValueConverter converter)
            : base(xmlName, isRequired, getter, converter) { }

        public override object Parse(XElement parentNode, ImmutableList<IValueConverter> converters)
        {
            parentNode.CheckNull("parentNode");

            XAttribute attribute;
            if (!TryGetValidatedAttribute(parentNode, out attribute))
                return null;

            bool supplyAttribute;
            IValueConverter selectedConverter;
            SelectConverter(attribute, converters, out selectedConverter, out supplyAttribute);

            return Parse(attribute, selectedConverter, supplyAttribute);
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

        private TPart Parse(XAttribute attribute, IValueConverter converter, bool supplyAttribute)
        {
            if (supplyAttribute)
            {
                return (TPart)converter.Convert(attribute, PartType);
            }

            return (TPart)converter.Convert(attribute.Value, PartType);
        }

        private void SelectConverter(XAttribute attribute, IEnumerable<IValueConverter> converters, out IValueConverter selectedConverter, out bool supplyAttribute)
        {
            if (Converter != null)
            {
                var canConvertResult = Converter.CanConvert(attribute, PartType);
                if (canConvertResult != CanConvertResult.ToObject)
                {
                    canConvertResult = Converter.CanConvert(attribute.Value, PartType);
                    if (canConvertResult != CanConvertResult.ToObject)
                        throw new InvalidOperationException("The specific converter for this part cannot convert the specified value.");

                    supplyAttribute = false;
                }
                else
                    supplyAttribute = true;

                selectedConverter = Converter;
                return;
            }

            foreach (var converter in converters)
            {
                var canConvertResult = converter.CanConvert(attribute, PartType);
                if (canConvertResult != CanConvertResult.ToObject)
                {
                    canConvertResult = converter.CanConvert(attribute.Value, PartType);
                    if (canConvertResult != CanConvertResult.ToObject)
                        continue;

                    supplyAttribute = false;
                }
                else
                    supplyAttribute = true;

                selectedConverter = converter;
                return;
            }

            throw new InvalidOperationException("Unable to serialize: Cannot find compatible converter. Value type: " + typeof(XAttribute).FullName + ", target type: " + PartType.FullName + ".");
        }

        private void SelectConverter(TPart value, IEnumerable<IValueConverter> converters, out IValueConverter selectedConverter, out CanConvertResult selectedConverterCanConvertResult)
        {
            if (Converter != null)
            {
                var canConvertResult = Converter.CanConvert(value, typeof(XAttribute));
                if (canConvertResult == CanConvertResult.CannotConvert)
                    throw new InvalidOperationException("The specific converter for this part cannot convert the specified value.");

                selectedConverterCanConvertResult = canConvertResult;
                selectedConverter = Converter;
                return;
            }

            foreach (var converter in converters)
            {
                var canConvertResult = converter.CanConvert(value, typeof(XAttribute));
                if (canConvertResult == CanConvertResult.CannotConvert)
                    continue;

                selectedConverterCanConvertResult = canConvertResult;
                selectedConverter = converter;
                return;
            }

            throw new InvalidOperationException("Unable to serialize: Cannot find compatible converter. Value type: " + value.GetType().FullName + ", target type: " + typeof(XAttribute).FullName + ".");
        }

        private XAttribute Serialize(TPart value, CanConvertResult canConvertResult, IValueConverter converter)
        {
            switch (canConvertResult)
            {
                case CanConvertResult.ToXAttribute:
                    {
                        return (XAttribute)converter.Convert(value, typeof(XAttribute));
                    }
                case CanConvertResult.ToXmlValue:
                    {
                        var xmlValue = converter.Convert(value, typeof(XAttribute));
                        var attr = new XAttribute(XmlName, xmlValue);
                        return attr;
                    }
                default:
                    throw new ArgumentOutOfRangeException("canConvertResult");
            }
        }

        private bool TryGetValidatedAttribute(XElement parentNode, out XAttribute attribute)
        {
            attribute = parentNode.Attribute(XmlName);
            if (attribute == null && IsRequired)
                throw new InvalidOperationException("Cannot parse part; the attribute is missing and the part is required.");

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