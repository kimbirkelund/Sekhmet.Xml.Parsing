using System;
using System.Collections.Immutable;
using System.Linq;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class Serializer<TSource> : ISerializer<TSource>
    {
        private readonly ImmutableList<IValueConverter> _converters;
        private readonly ImmutableList<IPart<TSource>> _parts;
        private readonly XName _rootXmlName;

        public Serializer(XName rootXmlName, ImmutableList<IValueConverter> converters, ImmutableList<IPart<TSource>> parts)
        {
            _rootXmlName = rootXmlName.CheckNull("rootXmlName");
            _converters = converters.CheckNull("converters");
            _parts = parts.CheckNull("parts")
                          .Check(p => p.All(ip => ip.HasGetter || !ip.IsRequired),
                                 p => new ArgumentException("Not all parts have getters or are not required.", "parts"));
        }

        public XElement Serialize(TSource source)
        {
            source.Check(s => !ReferenceEquals(s, null),
                         s => new ArgumentNullException("source"));

            return new XElement(_rootXmlName, from p in _parts
                                              where p.HasGetter
                                              select p.Serialize(source, _converters));
        }
    }
}