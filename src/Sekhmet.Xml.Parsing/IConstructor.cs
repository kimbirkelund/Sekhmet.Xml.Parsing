using System;
using System.Collections.Immutable;

namespace Sekhmet.Xml.Parsing
{
    public interface IConstructor<out T>
    {
        T Create(ImmutableList<object> values);

        ValidationResult ValidateByTypes(ImmutableList<Type> types);
        ValidationResult ValidateByValues(ImmutableList<object> values);
    }
}