using FluentAssertions;
using Moq;
using ProcessorLib.Contracts;
using ProcessorLib.Services;

namespace ProcessorTests.ProcessorLibTests;

public class BatchProcessorServiceTests
{
    [Fact]
    public async Task BatchProcessorService_EmptyJdList_ReturnsEmptyResult()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(new HashSet<string>());
        
        var mockMatch = new Mock<IMatchScorer>();
        
        var service = new BatchProcessorService(mockExtractor.Object, mockMatch.Object);
        
        // Act
        var result = await service.ProcessBatchAsync("Sample resume content", new List<string>(), CancellationToken.None);
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task BatchProcessorService_MultipleJdList_ReturnsAllOrdersByScoreDescending()
    {
        // Arrange
        var mockExtractor = new Mock<IKeywordExtractor>();
        mockExtractor.Setup(e => e.ExtractKeywords(It.IsAny<string>()))
            .Returns(new HashSet<string>());
        
        var score = new Queue<double>(new []{ 25.0, 85.0 }); // Simulate different scores for each JD
        var mockMatch = new Mock<IMatchScorer>();
        mockMatch.Setup(s => s.CalculateMatchScore(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(score.Dequeue); 
        mockMatch.Setup(s => s.FindMissingSkills(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string> { "Skill1", "Skill2" });
        mockMatch.Setup(s => s.FindStrongMatches(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new List<string> { "C#", "ASP.NET" });
        
        var service = new BatchProcessorService(mockExtractor.Object, mockMatch.Object);
        var jd = new List<string> { "Low Match JD Text", "High Match JD Text"  };
        
        // Act
        var result = await service.ProcessBatchAsync("Resume content", jd, CancellationToken.None);
        
        // Assert
        // result.Should().BeEquivalentTo(jd);
        result.Should().HaveCount(2);
        result.First().Score.Should().Be(85.0);
    }
}