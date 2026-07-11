using System.Text;
using ProcessorLib.Contracts;
using ProcessorLib.Models;

namespace ProcessorLib.Services;

public class MatchProcessorService : IMatchProcessorService
{
    private readonly IKeywordExtractor _extractor;
    private readonly IMatchScorer _scorer;
    public MatchProcessorService(IKeywordExtractor extractor, IMatchScorer scorer)
    {
        _extractor = extractor;
        _scorer = scorer;
    }

    public Task<MatchResult> ProcessAsync(string resumeText, string jdText, CancellationToken token)
    {
        return Task.Run(() =>
        {
            token.ThrowIfCancellationRequested();

            var resumeKeywords = _extractor.ExtractKeywords(resumeText);
            var jdKeywords = _extractor.ExtractKeywords(jdText);

            var score = _scorer.CalculateMatchScore(resumeKeywords, jdKeywords);
            var missing = _scorer.FindMissingSkills(resumeKeywords, jdKeywords);
            var strongMatches = _scorer.FindStrongMatches(resumeKeywords, jdKeywords);
            var tailored = GenerateTailoredResume(resumeText, strongMatches, missing);

            return new MatchResult
            {
                Score = score,
                JdKeywords = jdKeywords.Take(20).ToList(),
                MissingSkills = missing.Take(10).ToList(),
                TailoredResumeMarkdown = tailored
            };
        }, token);
    }
    
    private static string GenerateTailoredResume(
        string resume, 
        IEnumerable<string> strongMatches,
        List<string> missing)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Optimized Resume");
        sb.AppendLine();
        
        sb.AppendLine("## Strong Matches");
        foreach (var skill in strongMatches.Take(5))
            sb.AppendLine($"- **Matches JD:** '{skill}'");
        
        sb.AppendLine();
        sb.AppendLine("## Suggested Additions");
        foreach (var skill in missing.Take(5))
            sb.AppendLine($"- **ADD Bullet infusing:**  '{skill}'");
        
        sb.AppendLine();
        sb.AppendLine($"Original length: {resume.Length} chars");
        
        return sb.ToString();
    }
}