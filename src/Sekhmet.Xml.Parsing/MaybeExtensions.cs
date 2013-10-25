using System;

namespace Sekhmet.Xml.Parsing
{
    public static class MaybeExtensions
    {
        public static Maybe<T> Maybe<T>(this T value, bool defaultAsMissingForValueTypes = false, bool nullAsMissingForReferenceTypes = true)
        {
            if (defaultAsMissingForValueTypes && typeof(T).IsValueType && Equals(value, default(T)))
                return Parsing.Maybe<T>.Empty;
            if (nullAsMissingForReferenceTypes && typeof(T).IsClass && Equals(value, default(T)))
                return Parsing.Maybe<T>.Empty;

            return value;
        }

        public static Maybe<T> Maybe<T>(this T value, Func<T, bool> isMissing)
        {
            if (isMissing(value))
                return Parsing.Maybe<T>.Empty;

            return value;
        }
    }
}