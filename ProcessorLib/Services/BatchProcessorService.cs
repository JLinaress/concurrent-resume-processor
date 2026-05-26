using System.Collections.Concurrent;
using System.Text;
using ProcessorLib.Contracts;
using ProcessorLib.Models;

namespace ProcessorLib.Services;

public class BatchProcessorService : IBatchProcessorService
{
    private readonly KeywordExtractor _extractor;
    private readonly MatchScorer _scorer;
    public BatchProcessorService(KeywordExtractor extractor, MatchScorer scorer)
    {
        _extractor = extractor;
        _scorer = scorer;
    }
    
    public async Task<List<MatchResult>> ProcessBatchAsync(
        string resumeText, 
        List<string> jdTexts, 
        CancellationToken token)
     {
         var resumeKeywords = _extractor.ExtractKeywords(resumeText);
         var results = new ConcurrentBag<MatchResult>();
         var semaphore = new SemaphoreSlim(4, 4);  // Max 4 concurrent JDs (CPU-safe for Mac)

         var tasks = jdTexts.Select(async jdText =>
         {
             await semaphore.WaitAsync(token);
             try
             {
                 var jdKeywords = _extractor.ExtractKeywords(jdText);
                 var score = _scorer.CalculateMatchScore(resumeKeywords, jdKeywords);
                 var missing = _scorer.FindMissingSkills(resumeKeywords, jdKeywords);
                 var strongMatches = _scorer.FindStrongMatches(resumeKeywords, jdKeywords);
                 var tailored = GenerateTailoredResume(resumeText, jdKeywords, missing);
                 
                 results.Add(new MatchResult
                 {
                     Score = score,
                     JdKeywords = jdKeywords.Take(20).ToList(), //update this to have exact JD description 
                     MissingSkills = missing.Take(10).ToList(),
                     TailoredResumeMarkdown = tailored,
                 });
             }
             finally { semaphore.Release(); }
         });
         
         await Task.WhenAll(tasks);
         return results.OrderByDescending(r => r.Score).ToList();
     }
     
    private static string GenerateTailoredResume(
        string resume, 
        HashSet<string> strongMatches,
        List<string> missing)
    {
        var sb = new StringBuilder(); //($"# Optimized Resume (ACTUAL: {resume.Length} chars)\n\n");
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