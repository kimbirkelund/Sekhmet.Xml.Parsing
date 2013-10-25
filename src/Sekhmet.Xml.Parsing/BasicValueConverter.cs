using System;
using System.Collections.Immutable;
using System.Linq;
using System.Monads;
using System.Xml;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class BasicValueConverter : IValueConverter
    {
        private readonly ImmutableList<IConversionHelper> _helpers;

        public BasicValueConverter()
        {
            var builder = ImmutableList<IConversionHelper>.Empty.ToBuilder();

            builder.Add(GetConverter(v => v, sbyte.Parse));
            builder.Add(GetConverter(v => v, byte.Parse));
            builder.Add(GetConverter(v => v, char.Parse));
            builder.Add(GetConverter(v => v, short.Parse));
            builder.Add(GetConverter(v => v, ushort.Parse));
            builder.Add(GetConverter(v => v, int.Parse));
            builder.Add(GetConverter(v => v, uint.Parse));
            builder.Add(GetConverter(v => v, long.Parse));
            builder.Add(GetConverter(v => v, ulong.Parse));

            builder.Add(GetConverter(v => v, float.Parse));
            builder.Add(GetConverter(v => v, double.Parse));
            builder.Add(GetConverter(v => v, decimal.Parse));

            builder.Add(GetConverter(v => v, bool.Parse));

            // string
            builder.Add(GetConverter(v => v, v => v));

            builder.Add(GetConverter(v => v, ParseDateTime));
            builder.Add(GetConverter(v => v, ParseTimeSpan));
            builder.Add(GetConverter(v => v, Guid.Parse));

            _helpers = builder.ToImmutable();
        }

        public CanConvertResult CanConvert(object value, Type targetType)
        {
            if (value == null)
                return CanConvertResult.CannotConvert;

            if (targetType == typeof(XElement) || targetType == typeof(XAttribute))
            {
                var valueType = value.GetType();

                return _helpers.Any(h => h.Type.IsAssignableFrom(valueType))
                    ? CanConvertResult.ToXmlValue
                    : CanConvertResult.CannotConvert;
            }

            if (!(value is string))
                return CanConvertResult.CannotConvert;

            return _helpers.Any(h => targetType.IsAssignableFrom(h.Type))
                ? CanConvertResult.ToObject
                : CanConvertResult.CannotConvert;
        }

        public object Convert(object value, Type targetType)
        {
            CanConvert(value, targetType).Check(v => v != CanConvertResult.CannotConvert,
                                                _ => new ArgumentException("Cannot convert specified value.", "value"));

            if (targetType == typeof(XElement) || targetType == typeof(XAttribute))
            {
                var valueType = value.GetType();

                return _helpers.First(h => h.Type.IsAssignableFrom(valueType))
                               .ConvertToXml(value);
            }

            return _helpers.First(h => targetType.IsAssignableFrom(h.Type))
                           .ConvertFromXml((string)value);
        }

        private static IConversionHelper GetConverter<T>(Func<T, object> convertToXmlValue, Func<string, T> convertFromXml)
        {
            return new ConversionHelper<T>(convertToXmlValue, convertFromXml);
        }

        private static DateTime ParseDateTime(string value)
        {
            DateTime res;
            if (!DateTime.TryParse(value, out res))
                res = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
            return res;
        }

        private static TimeSpan ParseTimeSpan(string value)
        {
            TimeSpan res;
            if (!TimeSpan.TryParse(value, out res))
                res = XmlConvert.ToTimeSpan(value);
            return res;
        }

        private class ConversionHelper<T> : IConversionHelper
        {
            private readonly Func<string, T> _converterFromXml;
            private readonly Func<T, object> _converterToXml;

            public Type Type
            {
                get { return typeof(T); }
            }

            public ConversionHelper(Func<T, object> converterToXml, Func<string, T> converterFromXml)
            {
                _converterToXml = converterToXml.CheckNull("converterToXml");
                _converterFromXml = converterFromXml.CheckNull("converterFromXml");
            }

            public object ConvertFromXml(string value)
            {
                return _converterFromXml(value);
            }

            public object ConvertToXml(object value)
            {
                return _converterToXml((T)value);
            }
        }

        private interface IConversionHelper
        {
            Type Type { get; }

            object ConvertFromXml(string value);
            object ConvertToXml(object value);
        }
    }
}