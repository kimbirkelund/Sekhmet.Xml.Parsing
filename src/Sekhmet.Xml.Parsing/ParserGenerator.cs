using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Monads;
using System.Xml.Linq;

namespace Sekhmet.Xml.Parsing
{
    public class ParserGenerator<TResult> : IParserGenerator<TResult>
    {
        public static readonly ParserGenerator<TResult> Empty = new ParserGenerator<TResult>();

        private readonly ConstructorBase<TResult> _constructor;
        private readonly ImmutableList<IValueConverter> _converters;
        private readonly ImmutableList<IPart<TResult>> _parts;
        private readonly XName _rootXmlName;

        public ParserGenerator()
            : this(null, null, null, null) { }

        private ParserGenerator(IConstructor<TResult> constructor, ImmutableList<IPart<TResult>> parts, ImmutableList<IValueConverter> converters, XName rootXmlName)
        {
            _constructor = (constructor as ConstructorBase<TResult>) ?? constructor.With(c => new WrappingConstructor<TResult>(c));
            _parts = parts ?? ImmutableList<IPart<TResult>>.Empty;
            _converters = converters ?? ImmutableList<IValueConverter>.Empty;
            _rootXmlName = rootXmlName;
        }

        public IParserGenerator<TResult> AddConverters(IEnumerable<IValueConverter> converters)
        {
            return With(converters: _converters.AddRange(converters));
        }

        public IParserGenerator<TResult> AddPart(IPart<TResult> part)
        {
            return With(parts: _parts.Add(part).Maybe());
        }

        public ValidationResult CanCreateParser()
        {
            if (_rootXmlName == null)
                return ValidationResult.Failure("Root XML name not set.");
            if (_constructor == null)
                return ValidationResult.Failure("Constructor not set");

            _constructor.ValidateByParts(_parts).Check(vr => vr.IsSuccess,
                                                       vr => vr.ToException());

            return ValidationResult.Success;
        }

        public ValidationResult CanCreateSerializer()
        {
            if (_rootXmlName == null)
                return ValidationResult.Failure("Root XML name not set.");
            if (!_parts.All(p => p.HasGetter || !p.IsRequired))
                return ValidationResult.Failure("Some parts cannot be serialized.");

            return ValidationResult.Success;
        }

        public IParserGenerator<TResult> SetConstructor<TPart1>(Func<TPart1, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2>(Func<TPart1, TPart2, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3>(Func<TPart1, TPart2, TPart3, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4>(Func<TPart1, TPart2, TPart3, TPart4, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5>(Func<TPart1, TPart2, TPart3, TPart4, TPart5, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6>(Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7>(Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9, TPart10>(
            Func<TPart1, TPart2, TPart3, TPart4, TPart5, TPart6, TPart7, TPart8, TPart9, TPart10, TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor(Func<TResult> constructor)
        {
            return With(new DelegateConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetConstructor(IConstructor<TResult> constructor)
        {
            return With(new WrappingConstructor<TResult>(constructor.CheckNull("constructor")));
        }

        public IParserGenerator<TResult> SetRootXmlName(XName rootXmlName)
        {
            return With(rootXmlName: rootXmlName.CheckNull("rootXmlName"));
        }

        public IParser<TResult> ToParser()
        {
            CanCreateParser().Check(vr => vr.IsSuccess,
                                    vr => vr.ToException());

            return new Parser<TResult>(_rootXmlName, _constructor, _converters, _parts);
        }

        public ISerializer<TResult> ToSerializer()
        {
            CanCreateSerializer().Check(vr => vr.IsSuccess,
                                        vr => vr.ToException());

            return new Serializer<TResult>(_rootXmlName, _converters, _parts);
        }

        public ParserGenerator<TResult> With(Maybe<IConstructor<TResult>> constructor = default(Maybe<IConstructor<TResult>>),
                                             Maybe<ImmutableList<IPart<TResult>>> parts = default(Maybe<ImmutableList<IPart<TResult>>>),
                                             Maybe<ImmutableList<IValueConverter>> converters = default(Maybe<ImmutableList<IValueConverter>>),
                                             Maybe<XName> rootXmlName = (default(Maybe<XName>)))
        {
            return new ParserGenerator<TResult>(constructor.GetValueOrDefault(_constructor),
                                                parts.GetValueOrDefault(_parts),
                                                converters.GetValueOrDefault(_converters),
                                                rootXmlName.GetValueOrDefault(_rootXmlName));
        }
    }
}