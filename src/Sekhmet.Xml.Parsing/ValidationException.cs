using System;
using System.Monads;
using System.Runtime.Serialization;

namespace Sekhmet.Xml.Parsing
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; private set; }

        public ValidationException(ValidationResult validationResult)
            : base(validationResult.CheckNull("validationResult").Reason)
        {
            ValidationResult = validationResult.CheckNull("validationResult");
        }

        public ValidationException(string message, ValidationResult validationResult)
            : base(message)
        {
            ValidationResult = validationResult.CheckNull("validationResult");
        }

        public ValidationException(string message, Exception inner, ValidationResult validationResult)
            : base(message, inner)
        {
            ValidationResult = validationResult.CheckNull("validationResult");
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}