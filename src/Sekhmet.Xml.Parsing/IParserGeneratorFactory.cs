namespace Sekhmet.Xml.Parsing
{
    public interface IParserGeneratorFactory
    {
        /// <summary>
        /// Returns a generator for the specified type.
        /// </summary>
        IParserGenerator<TResult> Create<TResult>();

        /// <summary>
        /// Returns a generator for the specified type. 
        /// Where <see cref="Create{T}"/> is allowed to add whatever converters it would like 
        /// this method must return a clean generator.
        /// </summary>
        IParserGenerator<TResult> CreateClean<TResult>();
    }
}