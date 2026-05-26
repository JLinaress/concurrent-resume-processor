using ProcessorLib.Services;
using FluentAssertions;

namespace ProcessorTests.ProcessorLibTests;

public class KeywordExtractorTests
{
    [Theory]
    [InlineData("DOTNET", ".net")]
    [InlineData("CI/CD", "ci/cd")]
    [InlineData("Business", "business")]
    [InlineData("Redis", "redis")]
    [InlineData("Node.js", "node.js")]
    [InlineData(null, "")]
    public void ExtractKeywords_EdgeCases_ReturnsHandledExpectedly(string input, string expected)
    {
        // Arrange
        var extractor = new KeywordExtractor();
        
        // Act
        var result = extractor.ExtractKeywords(input);
        
        // Assert
        if (string.IsNullOrEmpty(expected))
            result.Should().BeEmpty();
        else
            result.Should().Contain(expected);
    }
    
    [Fact]
    public void ExtractKeywords_MixedInputsAndPunctuation_ReturnsExpectedNormalizedForm()
    {
        // Arrange
        var extractor = new KeywordExtractor();
        string input = "The resumes; for C# not F#!";
        
        // Act
        var result = extractor.ExtractKeywords(input);
        
        // Assert
        result.Should().BeEquivalentTo("resume", "c#");
        result.Should().Contain("resume");
        result.Should().Contain("c#");
    }
}