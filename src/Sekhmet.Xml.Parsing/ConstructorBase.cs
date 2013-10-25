using System;
using System.Collections.Immutable;
using System.Linq;
using System.Monads;

namespace Sekhmet.Xml.Parsing
{
    internal abstract class ConstructorBase<TResult> : IConstructor<TResult>
    {
        public abstract TResult Create(ImmutableList<object> values);

        public virtual ValidationResult ValidateByParts(ImmutableList<IPart<TResult>> parts)
        {
            return ValidateByTypes(parts.CheckNull("parts").Select(p => p.PartType).ToImmutableList());
        }

        public abstract ValidationResult ValidateByTypes(ImmutableList<Type> types);

        public virtual ValidationResult ValidateByValues(ImmutableList<object> values)
        {
            return ValidateByTypes(values.CheckNull("values")
                                         .Select(v => v.With(iv => iv.GetType()))
                                         .ToImmutableList());
        }
    }
}