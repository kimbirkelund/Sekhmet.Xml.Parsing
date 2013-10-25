using System;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public static class Part
    {
        public static Part<TParent> In<TParent>()
        {
            return Part<TParent>.Empty;
        }
    }

    public sealed class Part<TParent>
    {
        public static readonly Part<TParent> Empty = new Part<TParent>();

        private readonly IValueConverter _converter;
        private readonly bool _isAttribute;
        private readonly bool _isRequired;
        private readonly XName _xmlName;

        private Part() { }

        private Part(bool isRequired, bool isAttribute, XName xmlName, IValueConverter converter)
        {
            _isRequired = isRequired;
            _isAttribute = isAttribute;
            _xmlName = xmlName;
            _converter = converter;
        }

        public TypedPart<TParent, TPart> HasType<TPart>()
        {
            return With<TPart>();
        }

        public Part<TParent> IsAttribute()
        {
            return With(isAttribute: true);
        }

        public Part<TParent> IsElement()
        {
            return With(isAttribute: false);
        }

        public Part<TParent> IsNotRequired()
        {
            return With(isRequired: false);
        }

        public Part<TParent> IsRequired()
        {
            return With(isRequired: true);
        }

        public Part<TParent> With(Maybe<bool> isRequired = default(Maybe<bool>),
                                  Maybe<bool> isAttribute = default(Maybe<bool>),
                                  Maybe<XName> xmlName = default(Maybe<XName>),
                                  Maybe<IValueConverter> converter = default(Maybe<IValueConverter>))
        {
            return new Part<TParent>(isRequired.GetValueOrDefault(_isRequired),
                                     isAttribute.GetValueOrDefault(_isAttribute),
                                     xmlName.GetValueOrDefault(_xmlName),
                                     converter.GetValueOrDefault(_converter));
        }

        public TypedPart<TParent, TPart> With<TPart>(Maybe<bool> isRequired = default(Maybe<bool>),
                                                     Maybe<bool> isAttribute = default(Maybe<bool>),
                                                     Maybe<Func<TParent, Maybe<TPart>>> getter = default(Maybe<Func<TParent, Maybe<TPart>>>),
                                                     Maybe<XName> xmlName = default(Maybe<XName>),
                                                     Maybe<IValueConverter> converter = default(Maybe<IValueConverter>))
        {
            return TypedPart<TParent, TPart>.Empty.With(isRequired.GetValueOrDefault(_isRequired),
                                                        isAttribute.GetValueOrDefault(_isAttribute),
                                                        getter.GetValueOrDefault(),
                                                        xmlName.GetValueOrDefault(_xmlName),
                                                        converter.GetValueOrDefault(_converter).Maybe());
        }

        public Part<TParent> WithConverter(IValueConverter converter)
        {
            return With(converter: converter.Maybe());
        }

        public TypedPart<TParent, TPart> WithGetter<TPart>(Func<TParent, Maybe<TPart>> getter)
        {
            return With<TPart>(getter: getter);
        }

        public Part<TParent> WithXmlName(XName xmlName)
        {
            return With(xmlName: xmlName);
        }
    }
}