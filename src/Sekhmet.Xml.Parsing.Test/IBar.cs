using System;

namespace Sekhmet.Xml.Parsing.Test
{
    public interface IBar : IEquatable<IBar>
    {
        TimeSpan Duration { get; }
        Guid Id { get; }
        IFoo Parent { get; }
        long Value { get; }
        void Initialize(IFoo parent);
    }
}