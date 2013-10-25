using System;
using System.Collections.Immutable;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    internal abstract class PartBase<TParent, TPart> : IPart<TParent>
    {
        public IValueConverter Converter { get; private set; }
        public Func<TParent, Maybe<TPart>> Getter { get; private set; }

        public bool HasGetter
        {
            get { return Getter != null; }
        }

        public bool IsRequired { get; private set; }

        public Type PartType
        {
            get { return typeof(TPart); }
        }

        public XName XmlName { get; private set; }

        protected PartBase(XName xmlName, bool isRequired, Func<TParent, Maybe<TPart>> getter, IValueConverter converter)
        {
            IsRequired = isRequired;
            Getter = getter;
            Converter = converter;
            XmlName = xmlName;
        }

        public Maybe<TPart> GetValue(TParent parent)
        {
            HasGetter.Check(v => v, v => new InvalidOperationException("No getter available. Cannot serialize."));
            if (ReferenceEquals(parent, null))
                throw new ArgumentNullException("parent");

            return Getter(parent);
        }

        public abstract object Parse(XElement parentNode, ImmutableList<IValueConverter> converters);

        public abstract XObject Serialize(TParent parent, ImmutableList<IValueConverter> converters);
    }
}