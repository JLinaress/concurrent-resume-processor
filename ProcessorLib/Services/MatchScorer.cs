using ProcessorLib.Contracts;

namespace ProcessorLib.Services;

public class MatchScorer : IMatchScorer
{
    public double CalculateMatchScore(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords)
    {
        if (jdKeywords == null || resumeKeywords == null) return 0.0;
        
        var resumeFiltered = resumeKeywords.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var jdFiltered = jdKeywords.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
    
        if (jdFiltered.Count == 0) return 0.0;
        
        var matched = resumeFiltered.Intersect(jdFiltered, StringComparer.OrdinalIgnoreCase).ToList();
        var score = (double)matched.Count / jdFiltered.Count * 100.0;
    
        Console.WriteLine($"JD has {jdFiltered.Count} tech skills");
        Console.WriteLine($"You have {matched.Count} of those skills");
        Console.WriteLine($"You MATCH: {string.Join(", ", matched)}");
        Console.WriteLine($"You MISSING: {string.Join(", ", jdFiltered.Except(resumeFiltered, StringComparer.OrdinalIgnoreCase))}");
        Console.WriteLine($"SCORE: {Math.Round(score, 2)}%");
    
        return Math.Round(score, 2);
    }

    public List<string> FindMissingSkills(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords)
    {
        if (jdKeywords == null) return new List<string>();
        
        if (resumeKeywords == null) 
        {
            return jdKeywords
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.Trim().ToLower())
                .Take(20)
                .ToList();
        }
        
        var comparer = StringComparer.OrdinalIgnoreCase;
        var resumeFiltered = resumeKeywords.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim().ToLower()).ToHashSet(comparer);
 
        // Filter and trim JD keywords but preserve their original visual casing for output
        return jdKeywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim())
            .Except(resumeFiltered, comparer)
            .Take(20)
            .ToList();
    }

    public List<string> FindStrongMatches(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords)
    {
        if (jdKeywords == null || resumeKeywords == null) return new List<string>();
        
        var comparer = StringComparer.OrdinalIgnoreCase;
        var resumeFiltered = resumeKeywords.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim().ToLower()).ToHashSet(comparer);
        var jdFiltered = jdKeywords.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim().ToLower()).ToHashSet(comparer);

        return resumeFiltered
            .Intersect(jdFiltered, comparer)
            .Take(20)
            .ToList();
    }
}