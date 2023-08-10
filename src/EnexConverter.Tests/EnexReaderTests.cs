using Xunit.Abstractions;

namespace EnexConverter.Tests
{
    public class EnexReaderTests : TestBase
    {
        public EnexReaderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void EnexReader_Parses_XML_File()
        {
            var executionPath = PathHelpers.GetExecutingPath();
            
            // 1. arrange
            var sampleFile = Path.Combine(executionPath ?? Directory.GetCurrentDirectory(), $"Samples{Path.DirectorySeparatorChar}sample.enex");

            // 2. act
            var reader = new EnexReader();
            var parsed = reader.ParseEvernoteEnex(sampleFile);

            WriteJson(parsed);

            // 3. assert
            Assert.Equal(3, parsed.Note.Count());

            var note1 = parsed.Note.Single(x => x.Title == "Note1");
            Assert.NotNull(note1.Title);
            Assert.NotNull(note1.Noteattributes.Author);
            Assert.Null(note1.Tag);

            var note2 = parsed.Note.Single(x => x.Title == "Note2");
            Assert.NotNull(note2.Title);
            Assert.NotNull(note1.Noteattributes.Author);
            Assert.NotNull(note2.Tag);

        }

    }
}