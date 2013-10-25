using System;

namespace Sekhmet.Xml.Parsing.Test
{
    public interface IFoo:IEquatable<IFoo>
    {
        IBar Bar { get; }
        DateTime DateTime { get; }
        string Name { get; }
        int Value { get; }
    }
}