using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace Sekhmet.Xml.Parsing.Test
{
    [TestFixture]
    public class ParserGeneratorFactoryTest
    {
        [Test]
        public void TestXmlParserGenerator()
        {
            IParserGeneratorFactory g = CreateSut();

            var pb = g.Create<IFoo>()
                      .SetRootXmlName("foo")
                      .AddPart(Part.In<IFoo>()
                                   .HasType<string>()
                                   .WithXmlName("name")
                                   .IsRequired()
                                   .WithGetter(f => f.Name)
                                   .IsAttribute())
                      .AddPart(Part.In<IFoo>()
                                   .HasType<int>()
                                   .WithXmlName("value")
                                   .WithGetter(f => f.Value)
                                   .IsRequired()
                                   .IsAttribute())
                      .AddPart(Part.In<IFoo>()
                                   .HasType<IBar>()
                                   .IsElement()
                                   .WithXmlName("bar")
                                   .WithGetter(f => f.Bar)
                                   .IsRequired()
                                   .WithGenerator(g.Create<IBar>()
                                                   .SetRootXmlName("bar")
                                                   .AddPart(Part.In<IBar>()
                                                                .HasType<Guid>()
                                                                .IsAttribute()
                                                                .WithXmlName("id")
                                                                .IsRequired()
                                                                .WithGetter(b => b.Id))
                                                   .AddPart(Part.In<IBar>()
                                                                .HasType<TimeSpan>()
                                                                .WithXmlName("duration")
                                                                .IsAttribute()
                                                                .WithGetter(b => b.Duration))
                                                   .AddPart(Part.In<IBar>()
                                                                .HasType<long>()
                                                                .WithXmlName("value")
                                                                .IsRequired()
                                                                .IsAttribute()
                                                                .WithGetter(b => b.Value))
                                                   .SetConstructor<Guid, TimeSpan, long>((id, duration, value) => new Bar(id, value)
                                                   {
                                                       Duration = duration
                                                   })))
                      .AddPart(Part.In<IFoo>()
                                   .HasType<DateTime>()
                                   .WithXmlName("date-time")
                                   .IsAttribute()
                                   .WithGetter(f => f.DateTime))
                      .SetConstructor<string, int, IBar, DateTime>((name, value, bar, dateTime) =>
                      {
                          var foo = new Foo(name, value, bar)
                          {
                              DateTime = dateTime
                          };
                          return foo;
                      });

            var expected = new Foo("Some foo", 42, new Bar(Guid.NewGuid(), 24)
            {
                Duration = TimeSpan.FromSeconds(42)
            })
            {
                DateTime = DateTime.Now
            };


            var parser = pb.ToParser();
            var serializer = pb.ToSerializer();

            XElement serialized = serializer.Serialize(expected);
            Console.WriteLine(serialized);
            var actual = parser.Parse(serialized);

            Assert.AreEqual(expected, actual);
        }

        private IParserGeneratorFactory CreateSut()
        {
            return new ParserGeneratorFactory();
        }
    }
}