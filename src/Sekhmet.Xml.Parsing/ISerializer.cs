using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public interface ISerializer<in TSource>
    {
        XElement Serialize(TSource source);
    }
}