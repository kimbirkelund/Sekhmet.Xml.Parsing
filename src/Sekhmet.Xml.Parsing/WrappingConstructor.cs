using System;
using System.Collections.Immutable;
using System.Monads;

namespace Sekhmet.Xml.Parsing
{
    internal class WrappingConstructor<TResult> : ConstructorBase<TResult>
    {
        private readonly IConstructor<TResult> _constructor;

        public WrappingConstructor(IConstructor<TResult> constructor)
        {
            _constructor = constructor.CheckNull("constructor");
        }

        public override TResult Create(ImmutableList<object> values)
        {
            return _constructor.Create(values);
        }

        public override ValidationResult ValidateByTypes(ImmutableList<Type> types)
        {
            return _constructor.ValidateByTypes(types);
        }
    }
}