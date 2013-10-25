using System;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public interface IPart<in TParent>
    {
        /// <summary>
        /// Gets a value indicating whether it is possible to get the XML value from this instance. 
        /// If not serialization is not possible unless <see cref="IsRequired"/> is <c>false</c>.
        /// </summary>
        bool HasGetter { get; }

        /// <summary>
        /// Gets a value indicating whether this part if required. 
        /// In parsing required causes failure if XML member is missing, in serialization 
        /// causes failure if no getter or <see cref="GetValue"/> returns an optional without value.
        /// </summary>
        bool IsRequired { get; }

        Type PartType { get; }

        XName XmlName { get; }

        object Parse(XElement parentNode, ImmutableList<IValueConverter> converters);

        XObject Serialize(TParent parent, ImmutableList<IValueConverter> converters);
    }
}