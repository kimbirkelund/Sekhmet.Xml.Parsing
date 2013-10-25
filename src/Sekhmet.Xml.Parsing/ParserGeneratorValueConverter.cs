using System;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class ParserGeneratorValueConverter<T> : IValueConverter
    {
        private readonly IParserGenerator<T> _generator;

        public ParserGeneratorValueConverter(IParserGenerator<T> generator)
        {
            _generator = generator.CheckNull("generator");
        }

        public CanConvertResult CanConvert(object value, Type targetType)
        {
            if (value == null)
                return CanConvertResult.CannotConvert;
            if (targetType == null)
                return CanConvertResult.CannotConvert;

            if (targetType == typeof(XElement))
                return (value is T) && _generator.CanCreateSerializer()
                    ? CanConvertResult.ToXElement
                    : CanConvertResult.CannotConvert;

            if (targetType.IsAssignableFrom(typeof(T)))
                return value is XElement && _generator.CanCreateParser()
                    ? CanConvertResult.ToObject
                    : CanConvertResult.CannotConvert;

            return CanConvertResult.CannotConvert;
        }

        public object Convert(object value, Type targetType)
        {
            CanConvertResult canConvertResult = CanConvert(value, targetType);
            switch (canConvertResult)
            {
                case CanConvertResult.ToXElement:
                    return _generator.ToSerializer().Serialize((T)value);
                case CanConvertResult.ToObject:
                    return _generator.ToParser().Parse((XElement)value);
                default:
                    throw new ArgumentException("Specified arguments cannot be converter by this instance.");
            }
        }
    }
}