using System;
using System.Monads;

namespace Sekhmet.Xml.Parsing.Test
{
    public class Bar : IBar, IEquatable<Bar>
    {
        public TimeSpan Duration { get; set; }
        public Guid Id { get; private set; }
        public IFoo Parent { get; private set; }
        public long Value { get; private set; }

        public Bar(Guid id, long value)
        {
            Value = value;
            Id = id;
        }

        public bool Equals(IBar other)
        {
            return Equals(other as Bar);
        }

        public bool Equals(Bar other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!Duration.Equals(other.Duration)
                && Id.Equals(other.Id)
                && Value == other.Value)
            {
                return false;
            }

            if (ReferenceEquals(Parent, other.Parent))
                return true;
            if (ReferenceEquals(Parent, null) && ReferenceEquals(other.Parent, null))
                return true;
            if (ReferenceEquals(Parent, null) || ReferenceEquals(other.Parent, null))
                return true;

            return Parent.DateTime.Equals(other.Parent.DateTime)
                   && string.Equals(Parent.Name, other.Parent.Name)
                   && Parent.Value == other.Parent.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Bar);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Parent != null ? Parent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                return hashCode;
            }
        }

        public void Initialize(IFoo parent)
        {
            Parent.Check(p => p == null, p => new InvalidOperationException("Parent has already been set."));
            parent.CheckNull("parent")
                  .Check(p => p.Bar == this, p => new ArgumentException("Specified parent should refer to this instance.", "parent"));
            Parent = parent;
        }
    }
}