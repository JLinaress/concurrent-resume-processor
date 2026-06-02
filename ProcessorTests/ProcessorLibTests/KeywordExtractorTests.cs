using ProcessorLib.Services;
using FluentAssertions;

namespace ProcessorTests.ProcessorLibTests;

public class KeywordExtractorTests
{
    [Theory]
    [InlineData("DOTNET", ".net")]
    [InlineData("CI/CD", "ci/cd")]
    [InlineData("Business", "business")] // Ends in 'ss'
    [InlineData("Redis", "redis")]       // Ends in 'is'
    [InlineData("Node.js", "node.js")]
    [InlineData("[]", "")]
    [InlineData("developers", "developer")] // Standard plural stripped
    [InlineData("aws", "aws")]             // Short length protected
    [InlineData("ci/cds", "ci/cds")]       // Non-letter protected
    [InlineData("ci/cd,tcp/ip", "tcp/ip")]  // Glued via comma
    [InlineData("   ", "")]                 // Only whitespace
    [InlineData(null, "")]                  // Null handling
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
    public void ExtractKeywords_StopWordsAndDuplicates_FiltersThemOut()
    {
        // Arrange
        var extractor = new KeywordExtractor();
        
        // Act
        var result = extractor.ExtractKeywords("C# and the C# language is great");
        
        // Assert
        result.Should().BeEquivalentTo(new[] { "c#", "language", "great" });
    }

    [Fact]
    public void ExtractKeywords_MixedInputsAndPunctuation_ReturnsExpectedNormalizedForm()
    {
        var extractor = new KeywordExtractor();
        var result = extractor.ExtractKeywords("The resumes; for C# not F#!").ToList();
        
        result.Should().BeEquivalentTo(new[] { "resume", "c#" });
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
    public void ExtractKeywords_PureNumbers_HandlesAsExpected()
    {
        // Arrange
        var extractor = new KeywordExtractor();
    
        // Act
        var result = extractor.ExtractKeywords("Built in 2026 with 5000 lines of code").ToList();
    
        // Assert
        result.Should().Contain("line"); // Changed from "lines" to match your pluralization logic
        result.Should().NotContain("lines"); 
    
        result.Should().Contain("2026");
        result.Should().Contain("5000");
    }
}