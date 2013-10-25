using System;

namespace Sekhmet.Xml.Parsing.Test
{
    public class Foo : IFoo, IEquatable<Foo>
    {
        public IBar Bar { get; private set; }
        public DateTime DateTime { get; set; }
        public string Name { get; private set; }
        public int Value { get; private set; }

        public Foo(string name, int value, IBar bar)
        {
            Name = name;
            Value = value;
            Bar = bar;
            bar.Initialize(this);
        }

        public bool Equals(Foo other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Equals(Bar, other.Bar)
                   && DateTime.Equals(other.DateTime)
                   && string.Equals(Name, other.Name)
                   && Value == other.Value;
        }

        public bool Equals(IFoo other)
        {
            return Equals(other as Foo);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Foo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Bar != null ? Bar.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Value;
                return hashCode;
            }
        }
    }
}