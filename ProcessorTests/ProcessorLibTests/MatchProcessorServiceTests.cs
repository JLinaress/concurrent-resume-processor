using FluentAssertions;
using Moq;
using ProcessorLib.Contracts;
using ProcessorLib.Services;

namespace ProcessorTests.ProcessorLibTests;

public class MatchProcessorServiceTests
{
    [Fact]
    public async Task MatchProcessorService_WhenCanceled_ThrowsOperationCanceledException()
    {
        // Arrange
        var service = new MatchProcessorService(new Mock<IKeywordExtractor>().Object, new Mock<IMatchScorer>().Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        
        // Act && Assert
        await service.Awaiting(s => s.ProcessAsync("sample resume content", "sample JD content", cts.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }
    
    [Fact]
    public async Task MatchProcessorService_EmptyJdList_ReturnsEmptyResult()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(Array.Empty<string>());

        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(m => m.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(0);
        mockMatch.Setup(m => m.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string>());
        mockMatch.Setup(m => m.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string>());
        
        var service = new MatchProcessorService(mockExtractor.Object, mockMatch.Object);

        // Act
        var result = await service.ProcessAsync("sample resume content", "", CancellationToken.None);

        // Assert
        result.Score.Should().Be(0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().BeEmpty();
        result.TailoredResumeMarkdown.Should().Contain("Optimized Resume");
    }

    [Fact]
    public async Task MatchProcessorService_SingleJd_ReturnsMatchResult()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(new HashSet<string>());

        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(85.0);
        mockMatch.Setup(s => s.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string> { "Skill1", "Skill2" });
        mockMatch.Setup(s => s.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string> { "C#", "ASP.NET" });

        var service = new MatchProcessorService(mockExtractor.Object, mockMatch.Object);

        var jd = "High Match JD Text";

        // Act
        var result = await service.ProcessAsync("Resume content", jd, CancellationToken.None);

        // Assert
        result.Score.Should().Be(85.0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().Equal("Skill1", "Skill2");
        result.TailoredResumeMarkdown.Should().Contain("C#");
        result.TailoredResumeMarkdown.Should().Contain("ASP.NET");
    }
    
    [Fact]
    public async Task MatchProcessorService_NullInputs_ReturnsZeroScoreAndEmptyLists()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(new HashSet<string>());
    
        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(0.0);
        mockMatch.Setup(s => s.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string>());
        mockMatch.Setup(s => s.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string>());
    
        var service = new MatchProcessorService(mockExtractor.Object, mockMatch.Object);
    
        // Act
        var result = await service.ProcessAsync("", "", CancellationToken.None);
    
        // Assert
        result.Score.Should().Be(0.0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().BeEmpty();
        result.TailoredResumeMarkdown.Should().Contain("Optimized Resume");
    }
    
    [Fact]
    public async Task MatchProcessorService_WhiteSpaceOnlyInputs_ReturnsZeroScoreAndEmptyLists()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(new HashSet<string>());
        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(0.0);
        mockMatch.Setup(s => s.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string>());
        mockMatch.Setup(s => s.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string>());
        
        var service = new MatchProcessorService(mockExtractor.Object, mockMatch.Object);
        
        // Act
        var result = await service.ProcessAsync("sample resume content", null, CancellationToken.None);
        
        // Assert
        result.Score.Should().Be(0.0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().BeEmpty();
        result.TailoredResumeMarkdown.Should().Contain("Optimized Resume");
    }

    [Fact]
    public async Task MatchProcessorService_WhenScorerReturnsExcessiveSkills_ReturnsTruncatedLimitList()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(Enumerable.Range(1, 30).Select(i => $"Keyword {i}"));
        
        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(Enumerable.Range(1, 15).Select(i => $"MissingSkill {i}").ToList());
        mockMatch.Setup(s => s.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())) 
            .Returns(Enumerable.Range(1, 10).Select(i => $"StrongMatch {i}").ToList());
        
        var service = new MatchProcessorService(mockExtractor.Object, mockMatch.Object);
        
        // Act
        var result = await service.ProcessAsync("sample resume content", "jd", CancellationToken.None);
        
        // Assert
        result.JdKeywords.Should().HaveCount(20);
        result.MissingSkills.Should().HaveCount(10);
    }
}