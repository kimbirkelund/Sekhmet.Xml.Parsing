using System;
using System.Collections.Immutable;
using System.Linq;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class Parser<T> : IParser<T>
    {
        private readonly IConstructor<T> _constructor;
        private readonly ImmutableList<IValueConverter> _converters;
        private readonly ImmutableList<IPart<T>> _parts;
        private readonly XName _rootXmlName;

        public Parser(XName rootXmlName, IConstructor<T> constructor, ImmutableList<IValueConverter> converters, ImmutableList<IPart<T>> parts)
        {
            _rootXmlName = rootXmlName.CheckNull("rootXmlName");
            _constructor = constructor.CheckNull("constructor");
            _converters = converters.CheckNull("converters");
            _parts = parts.CheckNull("parts");
        }

        public T Parse(XElement element)
        {
            element.CheckNull("element")
                   .Check(e => e.Name == _rootXmlName,
                          e => new ArgumentException("Specified parameter has the wrong name.", "element"));

            var partValues = _parts.Select(part => part.Parse(element, _converters))
                                   .ToImmutableList();

            return _constructor.Create(partValues);
        }
    }
}