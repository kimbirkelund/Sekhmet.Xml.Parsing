namespace Sekhmet.Xml.Parsing
{
    public static class ParserGeneratorExtensions
    {
        public static IParserGenerator<TParent> AddConverters<TParent>(this IParserGenerator<TParent> generator, params IValueConverter[] converters)
        {
            return generator.AddConverters(converters);
        }

        public static IParserGenerator<TParent> AddPart<TParent, TPart>(this IParserGenerator<TParent> generator, TypedPart<TParent, TPart> part)
        {
            return generator.AddPart(part.ToPart());
        }

        public static IValueConverter AddPart<TParent>(this IParserGenerator<TParent> generator)
        {
            return new ParserGeneratorValueConverter<TParent>(generator);
        }
    }
}