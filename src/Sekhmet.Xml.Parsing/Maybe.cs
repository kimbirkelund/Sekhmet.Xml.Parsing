using System;
using System.Monads;

namespace Sekhmet.Xml.Parsing
{
    public struct Maybe<T>
    {
        public static readonly Maybe<T> Empty = default(Maybe<T>);

        public bool HasValue { get; private set; }

        public bool IsMissing
        {
            get { return !HasValue; }
        }

        public T Value { get; private set; }

        public Maybe(T value)
            : this()
        {
            Value = value;
            HasValue = true;
        }

        public T GetValueOrDefault(Func<T> defaultValueGenerator)
        {
            defaultValueGenerator.CheckNull("defaultValueGenerator");

            if (HasValue)
                return Value;

            return defaultValueGenerator();
        }

        public T GetValueOrDefault(T defaultValue = default(T))
        {
            return GetValueOrDefault(() => defaultValue);
        }

        public static implicit operator Maybe<T>(T value)
        {
            return new Maybe<T>(value);
        }
    }
}