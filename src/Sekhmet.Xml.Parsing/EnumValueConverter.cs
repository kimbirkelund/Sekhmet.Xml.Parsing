using System;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class EnumValueConverter : IValueConverter
    {
        public CanConvertResult CanConvert(object value, Type targetType)
        {
            if (value == null)
                return CanConvertResult.CannotConvert;

            if (targetType == typeof(XElement) || targetType == typeof(XAttribute))
            {
                var valueType = value.GetType();

                return valueType.IsEnum
                    ? CanConvertResult.ToXmlValue
                    : CanConvertResult.CannotConvert;
            }

            if (!(value is string))
                return CanConvertResult.CannotConvert;

            return targetType.IsEnum
                ? CanConvertResult.ToObject
                : CanConvertResult.CannotConvert;
        }

        public object Convert(object value, Type targetType)
        {
            CanConvert(value, targetType).Check(v => v != CanConvertResult.CannotConvert,
                                                _ => new ArgumentException("Cannot convert specified value.", "value"));

            if (targetType == typeof(XElement) || targetType == typeof(XAttribute))
                return value;


            return Enum.Parse(targetType, (string)value);
        }
    }
}