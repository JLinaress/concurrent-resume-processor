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
            .Returns(Enumerable.Range(1, 30).Select(i => $"Keyword {i}").ToList());
    
        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(100.0);
        mockMatch.Setup(s => s.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(Enumerable.Range(1, 15).Select(i => $"MissingSkill{i}").ToList()); // 15 skills
        mockMatch.Setup(s => s.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())) 
            .Returns(Enumerable.Range(1, 10).Select(i => $"StrongMatch{i}").ToList());  // 10 matches
    
        var service = new MatchProcessorService(mockExtractor.Object, mockMatch.Object);
    
        // Act
        var result = await service.ProcessAsync("sample resume content", "jd", CancellationToken.None);
    
        // Assert
        result.JdKeywords.Should().HaveCount(20);
        result.MissingSkills.Should().HaveCount(10);

        // NEW BOUNDARY CHECKS: Verifies that GenerateTailoredResume honors its Take(5) loops
        result.TailoredResumeMarkdown.Should().Contain("StrongMatch5");
        result.TailoredResumeMarkdown.Should().NotContain("StrongMatch6"); // Truncated!

        result.TailoredResumeMarkdown.Should().Contain("MissingSkill5");
        result.TailoredResumeMarkdown.Should().NotContain("MissingSkill6"); // Truncated!
    }
    
    [Fact]
    public async Task ProcessAsync_WhenCancellationTokenIsTriggered_AbortsImmediatelyAndThrowsException()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        var mockScorer = new Mock<IMatchScorer>();
        var service = new MatchProcessorService(mockExtractor.Object, mockScorer.Object);

        // Create a token and cancel it immediately before sending it to the service
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await service.ProcessAsync("Sample resume", "Sample JD", cts.Token);

        // Assert: Ensure it cleanly throws the standard cancellation exception
        await act.Should().ThrowAsync<OperationCanceledException>();

        // Ensure the service stopped before it ever reached your heavy dependency methods
        mockExtractor.Verify(e => e.ExtractKeywords(It.IsAny<string>()), Times.Never);
        mockScorer.Verify(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }
}