using FluentAssertions;
using ProcessorLib.Services;

namespace ProcessorTests.ProcessorLibTests;

public class MatchScorerTests
{
        [Theory]
        [InlineData(new[] { "c#", "sql" }, new[] { "c#", "sql" }, 100.00)] // Perfect match
        [InlineData(new[] { "c#", "python", "sql" }, new[] { "c#", "asp.net", "sql" }, 50.00)] // Partial match
        [InlineData(new[] { "python", "django" }, new[] { "c#", "asp.net", "sql" }, 0.00)] // No match  
        public void CalculateMatchScore_PerfectMatch_Returns100(string[] resume, string[] jd, double expected)
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(resume);
            var jdKeyword = new HashSet<string>(jd);
            
            // Act
            var result = scorer.CalculateMatchScore(resumeKeyword, jdKeyword);
            
            // Assert 
            result.Should().Be(expected);
        }
        
        [Theory]
        [InlineData(new[] { "C#" }, new[]  {"c#"}, 100.00)] // Case Insensitive
        [InlineData(new[] { "c#", "sql" }, new string[] { }, 0.0)] // Empty JD
        [InlineData(new[] { "sql", "sql", "SQL" }, new[] { "sql" }, 100.00)] // Duplicates
        [InlineData(new[] { "a" }, new[] { "a", "b", "c" }, 33.33)]
        [InlineData(new[] { "c#", null, "sql" }, new[] { "c#", "sql" }, 100.00)] // Res has null value
        [InlineData(new string[] { }, new[] { "sql" }, 0.0)] // Empty resume (no elements)
        [InlineData(new string[] { }, new string[] { }, 0.0)] // Both empty
        public void CalculateMatchScore_Rounding_ReturnsExpected(string[] resume, string[] jd, double expected)
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(resume);
            var jdKeyword = new HashSet<string>(jd);
            
            // Act
            var result = scorer.CalculateMatchScore(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().Be(expected);
        }
        
        [Fact]
        public void FindStrongMatches_NoOverlaps_ReturnsEmpty()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(new[] { "c#", "sql" });
            var jdKeyword = new HashSet<string>(new[] { "F#", "NoSQL" });
            
            // Act
            var result = scorer.FindStrongMatches(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindStrongMatches_MoreThan20Overlaps_ReturnsExactly20()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(Enumerable.Range(1, 25).Select(i => $"skill{i}"));
            var jdKeyword = new HashSet<string>(Enumerable.Range(1, 25).Select(i => $"skill{i}"));

            // Act
            var result = scorer.FindStrongMatches(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().HaveCount(20);
        }

        [Fact]
        public void FindMissingSkills_BothEmpty_ReturnsEmpty()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>();
            var jdKeyword = new HashSet<string>();
            
            // Act
            var result = scorer.FindMissingSkills(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().BeEmpty();
        }
        [Fact]
        public void FindMissingSkills_ResumeHasAllSkills_ReturnsEmpty()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(new[] { "c#", "sql" });
            var jdKeyword = new HashSet<string>(new string[] { });
            
            // Act
            var result = scorer.FindMissingSkills(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindMissingSkills_MoreThan20Missing_ReturnsExpectedAmount()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(new[] { "c#", "sql" });
            var jdKeyword = new HashSet<string>(new[] { "C#", "NoSQL", "Redis", "SQL"});
            
            // Act
            var result = scorer.FindMissingSkills(resumeKeyword, jdKeyword);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public void FindMissingSkills_ReturnsOriginalJDKeywordCasing()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = new HashSet<string>(new[] { "c#" });
            var jdKeyword = new HashSet<string>(new[] { "C#", "SQL", "ASP.NET" });
            
            // Act
            var result = scorer.FindMissingSkills(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().ContainEquivalentOf("SQL");
            result.Should().ContainEquivalentOf("ASP.NET");
        }
}