using System;

namespace Sekhmet.Xml.Parsing
{
    public interface IValueConverter
    {
        CanConvertResult CanConvert(object value, Type targetType);
        object Convert(object value, Type targetType);
    }
}