using System;
using System.Monads;

namespace Sekhmet.Xml.Parsing
{
    public class ValidationResult
    {
        public static readonly ValidationResult Success = new ValidationResult();

        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets the reason why validation failed. Is always null if success.
        /// </summary>
        public string Reason { get; private set; }

        private ValidationResult()
        {
            IsSuccess = true;
        }

        private ValidationResult(string reason)
        {
            Reason = reason.CheckNull("reason");
            IsSuccess = false;
        }

        public Exception ToException()
        {
            return new ValidationException(this);
        }

        public static ValidationResult Failure(string reason)
        {
            return new ValidationResult(reason);
        }

        public static implicit operator bool(ValidationResult validationResult)
        {
            return validationResult.IsSuccess;
        }
    }
}