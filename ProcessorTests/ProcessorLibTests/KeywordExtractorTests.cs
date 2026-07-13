using ProcessorLib.Services;
using FluentAssertions;

namespace ProcessorTests.ProcessorLibTests;

//fix tests
public class KeywordExtractorTests
{
    [Theory]
    [InlineData("DOTNET", "dotnet")]         // Exists in TechKeywords
    [InlineData("CI/CD", "ci/cd")]           // Exists in TechKeywords
    [InlineData("Redis", "redis")]           // Exists in TechKeywords
    [InlineData("Node.js", "node.js")]       // Exists in TechKeywords
    [InlineData("aws", "aws")]               // Exists in TechKeywords
    [InlineData("ci/cd,tcp/ip", "tcp/ip")]   // Comma is replaced by space in CleanText, "tcp/ip" matches
    [InlineData("   ", "")]                  // Only whitespace handled safely
    [InlineData(null, "")]                   // Null handled safely
    public void ExtractKeywords_EdgeCases_ReturnsHandledExpectedly(string? input, string expected)
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

    [Theory]
    [InlineData("DOTNET", ".net")]
    [InlineData("ts", "typescript")]
    [InlineData("csharp", "c#")]
    [InlineData("business...", "business")]
    [InlineData("-redis-", "redis")]
    [InlineData("/aws/", "aws")]
    public void NormalizeToken_VariousTokens_ReturnsExpectedNormalizedForm(string? input, string expected)
    {
        // Arrange
        var extractor = new KeywordExtractor();
        
        // Act
        var result = extractor.NormalizeToken(input);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void ExtractKeywords_ExtractDuplicates_FiltersThemOut()
    {
        // Arrange
        var extractor = new KeywordExtractor();
        
        // Act
        var result = extractor.ExtractKeywords("C# and the C# language is great");
        
        // Assert
        result.Should().BeEquivalentTo(new[] { "c#" });
    }

    [Fact]
    public void ExtractKeywords_MixedInputsAndPunctuation_ReturnsExpectedNormalizedForm()
    {
        var extractor = new KeywordExtractor();
        var result = extractor.ExtractKeywords("The resumes; for C# not F#!").ToList();
        
        result.Should().BeEquivalentTo(new[] { "c#", "f#" });
    }
    
    [Fact]
    public void ExtractKeywords_AllowedPunctuationAtWordBoundaries_StripsTrailingPunctuation()
    {
        var extractor = new KeywordExtractor();
    
        // Act & Assert 
        var result = extractor.ExtractKeywords("I program in C#.").ToList();
        result.Should().Contain("c#"); 
        result.Should().NotContain("c#."); // Bug check: makes sure trailing punctuation isn't glued
    }

    [Fact]
    public void ExtractKeywords_TokensGluedByPeriods_SplitsThemCorrectly()
    {
        var extractor = new KeywordExtractor();
    
        // Act
        var result = extractor.ExtractKeywords("Using with.net framework").ToList();
    
        // Assert
        result.Should().Contain(".net");
        result.Should().NotContain("with.net"); // Bug check: ensures it didn't glue 'with' and '.net'
    }

    [Fact]
    public void ExtractKeywords_PureNumbers_ReturnsEmptySafely()
    {
        // Arrange
        var extractor = new KeywordExtractor();
    
        // Act
        var result = extractor.ExtractKeywords("Built in 2026 with 5000 lines of code").ToList();
    
        // Assert
        result.Should().BeEmpty(); // Pure numbers should not be extracted as keywords
    }
}