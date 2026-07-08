using FluentAssertions;
using Moq;
using ProcessorLib.Contracts;
using ProcessorLib.Services;

namespace ProcessorTests.ProcessorLibTests;

public class BatchProcessorServiceTests
{
    [Fact]
    public void BatchProcessorService_EmptyJdList_ReturnsEmptyResult()
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
        
        var service = new BatchProcessorService(mockExtractor.Object, mockMatch.Object);

        // Act
        var result =
            service.ProcessAsync("sample resume content", "", CancellationToken.None);

        // Assert
        result.Score.Should().Be(0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().BeEmpty();
        result.TailoredResumeMarkdown.Should().Contain("Optimized Resume");
    }

    [Fact]
    public void BatchProcessorService_SingleJd_ReturnsMatchResult()
    {
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

        var service = new BatchProcessorService(mockExtractor.Object, mockMatch.Object);

        var jd = "High Match JD Text";

        var result = service.ProcessAsync("Resume content", jd, CancellationToken.None);

        result.Score.Should().Be(85.0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().Equal("Skill1", "Skill2");
        result.TailoredResumeMarkdown.Should().Contain("C#");
        result.TailoredResumeMarkdown.Should().Contain("ASP.NET");
    }
    
    [Fact]
    public void BatchProcessorService_NullInputs_ReturnsZeroScoreAndEmptyLists()
    {
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

        var service = new BatchProcessorService(mockExtractor.Object, mockMatch.Object);

        var result = service.ProcessAsync("", "", CancellationToken.None);

        result.Score.Should().Be(0.0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().BeEmpty();
        result.TailoredResumeMarkdown.Should().Contain("Optimized Resume");
    }
    
    [Fact]
    public void BatchProcessorService_WhiteSpaceOnlyInputs_ReturnsZeroScoreAndEmptyLists()
    {
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(new HashSet<string>());
        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(0.0);
        var service = new BatchProcessorService(mockExtractor.Object, mockMatch.Object);
        var result = service.ProcessAsync("sample resume content", "", CancellationToken.None);
        result.Score.Should().Be(0.0);
        result.JdKeywords.Should().BeEmpty();
        result.MissingSkills.Should().BeEmpty();
        result.TailoredResumeMarkdown.Should().Contain("Optimized Resume");
    }
}