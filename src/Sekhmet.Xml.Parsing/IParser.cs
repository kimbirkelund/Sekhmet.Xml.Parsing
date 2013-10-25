using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public interface IParser<out TResult>
    {
        TResult Parse(XElement element);
    }
}