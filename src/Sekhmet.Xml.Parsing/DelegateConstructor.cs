using System;
using System.Collections.Immutable;
using System.Linq;
using System.Monads;

namespace Sekhmet.Xml.Parsing
{
    internal class DelegateConstructor<TResult> : ConstructorBase<TResult>
    {
        private readonly Delegate _constructor;

        public DelegateConstructor(Delegate constructor)
        {
            _constructor = constructor.CheckNull("constructor")
                                      .Check(d => d.Method.ReturnType == typeof(TResult),
                                             d => new ArgumentException("Specified parameters return type is incorrect.", "constructor"));
        }

        public override TResult Create(ImmutableList<object> values)
        {
            ValidateByValues(values).Check(vr => vr.IsSuccess,
                                           vr => vr.ToException());

            return (TResult)_constructor.DynamicInvoke(values.ToArray());
        }

        public override ValidationResult ValidateByTypes(ImmutableList<Type> types)
        {
            var constructorParameterTypes = _constructor.Method
                                                        .GetParameters()
                                                        .Select(p => p.ParameterType)
                                                        .ToImmutableList();

            if (constructorParameterTypes.Count != types.Count)
                return ValidationResult.Failure("Parameter count mismatch.");

            var zipped = types.Zip(constructorParameterTypes, (t1, t2) => new { ActualType = t1, ConstructorType = t2 });

            return zipped.All(pair => pair.ConstructorType.IsAssignableFrom(pair.ActualType))
                ? ValidationResult.Success
                : ValidationResult.Failure("Parameter type mismatch.");
        }
    }
}