using FluentAssertions;
using ProcessorLib.Services;

namespace ProcessorTests.ProcessorLibTests;

public class MatchScorerTests
{
        [Theory]
        [InlineData(new[] { "c#", "sql" }, new[] { "c#", "sql" }, 100.00)] // Perfect match
        [InlineData(new[] { "c#", "python", "sql" }, new[] { "c#", "asp.net", "sql" }, 66.67)] // Partial match
        [InlineData(new[] { "python", "django" }, new[] { "c#", "asp.net", "sql" }, 0.00)] // No match  
        public void CalculateMatchScore_PerfectMatch_Returns100(string[] resume, string[] jd, double expected)
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.CalculateMatchScore(resume, jd);
            
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
        [InlineData(new[] { "   " }, new[] { "sql" }, 0.0)] // Whitespace-only strings in resume
        [InlineData(new[] { "sql" }, new[] { "  sql  " }, 100.00)] // Leading/trailing spaces in JD
        [InlineData(new[] { "c#", "sql" }, new[] { "C#", "SQL" }, 100.00)] // Mixed casing in both collections
        public void CalculateMatchScore_Rounding_ReturnsExpected(string[] resume, string[] jd, double expected)
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.CalculateMatchScore(resume, jd);
            
            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void CalculateMatchScore_NullInputs_ReturnsZerosSafely()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.CalculateMatchScore(null!, null!);
            
            // Assert
            result.Should().Be(0.0);
        }

        [Fact]
        public void FindMissingSkills_NBothEmpty_ReturnsEmpty()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindMissingSkills([], []);
            
            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindMissingSkills_NullInputs_ReturnsExpectedValuesSafely()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindMissingSkills(null!, new []{ "c#", "sql" });
            
            // Assert
            // If JD is null, nothing can be missing
            scorer.FindMissingSkills(new []{ "c#", "sql" }, null!).Should().BeEmpty();
            // If resume is null, all clean JD skills are missing
            result.Should().BeEquivalentTo(new[] { "c#", "sql" });
        }
        
        [Fact]
        public void FindMissingSkills_ResumeHasAllSkills_ReturnsEmpty()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindMissingSkills(new []{ "sQl", "csharp" }, []);
            
            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindMissingSkills_MoreThan20Missing_ReturnsExpectedAmount()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = Enumerable.Range(1, 5).Select(i => $"skill{i}").ToArray();
            var jdKeyword = Enumerable.Range(1, 30).Select(i => $"skill{i}").ToArray();
            
            // Act
            var result = scorer.FindMissingSkills(resumeKeyword, jdKeyword);

            // Assert
            result.Should().HaveCount(20);
        }

        [Fact]
        public void FindMissingSkills_ReturnsOriginalJDKeywordCasing()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindMissingSkills(new[] { "c#" }, new[] { "C#", "SQL", "ASP.NET" });
            
            // Assert
            result.Should().ContainEquivalentOf("SQL");
            result.Should().ContainEquivalentOf("ASP.NET");
        }
        
        [Fact]
        public void FindStrongMatches_NoOverlaps_ReturnsEmpty()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindStrongMatches(["python", "django"], ["c#", "asp.net", "sql"]);
            
            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void FindStrongMatches_MoreThan20Overlaps_ReturnsExactly20()
        {
            // Arrange
            var scorer = new MatchScorer();
            var resumeKeyword = Enumerable.Range(1, 30).Select(i => $"skill{i}").ToArray();
            var jdKeyword = Enumerable.Range(1, 30).Select(i => $"skill{i}").ToArray();

            // Act
            var result = scorer.FindStrongMatches(resumeKeyword, jdKeyword);
            
            // Assert
            result.Should().HaveCount(20);
        }

        [Fact]
        public void FindStrongMatches_MixedCasingAndSpaces_ReturnsMatchesCorrectly()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindStrongMatches(new []{"c#", "SQL"}, new []{"C#", "sql"});;
            
            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo("c#", "sql");
        }

        [Fact]
        public void FindStrongMatches_NullInputs_ReturnsZerosSafely()
        {
            // Arrange
            var scorer = new MatchScorer();
            
            // Act
            var result = scorer.FindStrongMatches(null!, null!);
            
            // Assert
            result.Should().BeEmpty();
        }
}