using System.ComponentModel;

namespace Sekhmet.Xml.Parsing
{
    public enum CanConvertResult
    {
        /// <summary>
        /// Indicates that this converter cannot convert the given value/type combination.
        /// </summary>
        CannotConvert = 0,

        /// <summary>
        /// Indicates that this converter can convert the given value/type combination and will produce an XElement.
        /// This value is only valid if targetType is XElement, as it would otherwise not be applicable in the context it needs be placed.
        /// </summary>
        ToXElement,

        /// <summary>
        /// Indicates that this converter can convert the given value/type combination and will produce an XAttribute.
        /// This value is only valid if targetType is XElement, as it would otherwise not be applicable in the context it needs be placed.
        /// </summary>
        ToXAttribute,

        /// <summary>
        /// Indicates that this converter can convert the given value/type combination and will produce a value that can be put into either an XElement or XAttribute, 
        /// taking targetType into account - e.g. if targetType is XAttribute the converter should not return an XCData; which would be fine if targetType is XElement.
        /// </summary>
        ToXmlValue,

        /// <summary>
        /// Indicates that this converter can convert the given value/type combination and will product an object that is compatible with targetType.
        /// </summary>
        ToObject
    }
}