using System;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class TypedPart<TParent, TPart>
    {
        public static readonly TypedPart<TParent, TPart> Empty = new TypedPart<TParent, TPart>();

        private readonly IValueConverter _converter;
        private readonly Func<TParent, Maybe<TPart>> _getter;
        private readonly bool _isAttribute;
        private readonly bool _isRequired;
        private readonly XName _xmlName;

        public TypedPart() { }

        private TypedPart(bool isRequired, bool isAttribute, Func<TParent, Maybe<TPart>> getter, XName xmlName, IValueConverter converter)
        {
            _isRequired = isRequired;
            _isAttribute = isAttribute;
            _getter = getter;
            _xmlName = xmlName;
            _converter = converter;
        }

        public TypedPart<TParent, TPart> IsAttribute()
        {
            return With(isAttribute: true);
        }

        public TypedPart<TParent, TPart> IsElement()
        {
            return With(isAttribute: false);
        }

        public TypedPart<TParent, TPart> IsRequired()
        {
            return With(isRequired: true);
        }

        public IPart<TParent> ToPart()
        {
            _xmlName.CheckNull(() => new InvalidOperationException("Method cannot be called until part has an XML name, as neither serialization or parsing is possible without."));

            if (_isAttribute)
                return new AttributePart<TParent, TPart>(_xmlName, _isRequired, _getter, _converter);

            return new ElementPart<TParent, TPart>(_xmlName, _isRequired, _getter, _converter);
        }

        public TypedPart<TParent, TPart> With(Maybe<bool> isRequired = default(Maybe<bool>),
                                              Maybe<bool> isAttribute = default(Maybe<bool>),
                                              Maybe<Func<TParent, Maybe<TPart>>> getter = default(Maybe<Func<TParent, Maybe<TPart>>>),
                                              Maybe<XName> xmlName = default(Maybe<XName>),
                                              Maybe<IValueConverter> converter = default(Maybe<IValueConverter>))
        {
            return new TypedPart<TParent, TPart>(isRequired.GetValueOrDefault(_isRequired),
                                                 isAttribute.GetValueOrDefault(_isAttribute),
                                                 getter.GetValueOrDefault(_getter),
                                                 xmlName.GetValueOrDefault(_xmlName),
                                                 converter.GetValueOrDefault(_converter));
        }

        public TypedPart<TParent, TPart> WithConverter(IValueConverter converter)
        {
            return With(converter: converter.Maybe());
        }

        public TypedPart<TParent, TPart> WithGetter(Func<TParent, Maybe<TPart>> getter)
        {
            return With(getter: getter);
        }

        public TypedPart<TParent, TPart> WithXmlName(XName xmlName)
        {
            return With(xmlName: xmlName);
        }
    }
}