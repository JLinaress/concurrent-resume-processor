using ProcessorLib.Services;

namespace ProcessorTests;

public class DocumentParserTests
{
    [Fact]
    public void ParsePdf_ReturnsTexts()
    {
        var text = DocumentParser.ParsePdf("TestData/Resume.pdf");
        
        Assert.NotNull(text);
        Assert.True(text.Length > 10);
        Assert.Contains("software", text.ToLower());
    }

    [Fact]
    public void ParseText_ReturnsTexts()
    {
        var text = DocumentParser.ParseWord("TestData/WordResume.docx");
        
        Assert.NotNull(text);
        Assert.True(text.Length > 10);
        Assert.Contains("c#", text.ToLower());
    }
}