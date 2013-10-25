using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public interface IParserGenerator<TResult>
    {
        IParserGenerator<TResult> AddConverters(IEnumerable<IValueConverter> converters);

        IParserGenerator<TResult> AddPart(IPart<TResult> part);

        ValidationResult CanCreateParser();
        ValidationResult CanCreateSerializer();

        IParserGenerator<TResult> SetConstructor(
            Func<TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1>(
            Func<TPart1, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2>(
            Func<TPart1, TPart2, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3>(
            Func<TPart1, TPart2, TPart3, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4>(
            Func<TPart1, TPart2, TPart3, TPart4, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9, TResult> constructor);

        IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9, TPart10>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9, TPart10, TResult> constructor);

        IParserGenerator<TResult> SetConstructor(IConstructor<TResult> constructor);

        IParserGenerator<TResult> SetRootXmlName(XName rootXmlName);

        IParser<TResult> ToParser();
        ISerializer<TResult> ToSerializer();
    }
}