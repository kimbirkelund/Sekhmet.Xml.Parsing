using System;

namespace Sekhmet.Xml.Parsing
{
    public static class PartExtensions
    {
        public static TypedPart<TParent, TPart> WithGenerator<TParent, TPart>(this Part<TParent> part, IParserGenerator<TPart> generator)
        {
            return part.HasType<TPart>()
                       .WithConverter(new ParserGeneratorValueConverter<TPart>(generator));
        }

        public static TypedPart<TParent, TPart> WithGenerator<TParent, TPart>(this TypedPart<TParent, TPart> part, IParserGenerator<TPart> generator)
        {
            return part.WithConverter(new ParserGeneratorValueConverter<TPart>(generator));
        }

        public static TypedPart<TParent, TPart> WithGetter<TParent, TPart>(this Part<TParent> part, Func<TParent, TPart> getter)
        {
            return part.WithGetter(p => getter(p).Maybe());
        }

        public static TypedPart<TParent, TPart> WithGetter<TParent, TPart>(this TypedPart<TParent, TPart> part, Func<TParent, TPart> getter)
        {
            return part.WithGetter(new Func<TParent, Maybe<TPart>>(p => getter(p).Maybe()));
        }
    }
}