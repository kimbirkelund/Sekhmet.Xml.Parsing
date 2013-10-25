using System.Collections.Immutable;

namespace Sekhmet.Xml.Parsing
{
    public class ParserGeneratorFactory : IParserGeneratorFactory
    {
        private readonly ImmutableList<IValueConverter> _converters;

        public ParserGeneratorFactory(ImmutableList<IValueConverter> converters)
        {
            _converters = converters ?? ImmutableList<IValueConverter>.Empty;
        }

        public ParserGeneratorFactory()
            : this(GetStandardValueConverters()) { }

        public IParserGenerator<TResult> Create<TResult>()
        {
            return ParserGenerator<TResult>.Empty.AddConverters(_converters);
        }

        public IParserGenerator<TResult> CreateClean<TResult>()
        {
            return ParserGenerator<TResult>.Empty;
        }

        private static ImmutableList<IValueConverter> GetStandardValueConverters()
        {
            var vcs = ImmutableList<IValueConverter>.Empty.ToBuilder();

            vcs.Add(new EnumValueConverter());
            vcs.Add(new BasicValueConverter());

            return vcs.ToImmutable();
        }
    }
}