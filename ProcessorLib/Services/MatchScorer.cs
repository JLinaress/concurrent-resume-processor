using ProcessorLib.Contracts;

namespace ProcessorLib.Services;

public class MatchScorer : IMatchScorer
{
    public double CalculateMatchScore(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords)
    {
        // Fix: Guard against null input right away to prevent NullReferenceException
        if (jdKeywords == null || resumeKeywords == null) return 0.0;

        var comparer = StringComparer.OrdinalIgnoreCase;
    
        // Select(k => k.Trim()) removes leading/trailing spaces before hashing
        var resumeFiltered = resumeKeywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim())
            .ToHashSet(comparer);

        var jdFiltered = jdKeywords
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim())
            .ToHashSet(comparer);
        
        if (jdFiltered.Count == 0) return 0.0;
    
        // Count how many JD words are present in the resume
        var intersectionCount = resumeFiltered.Intersect(jdFiltered, comparer).Count();
    
        // Calculate coverage: (Matched Words / Total JD Words)
        var coverage = (double)intersectionCount / jdFiltered.Count;
    
        return Math.Round(coverage * 100.0, 2);
    }

    public List<string> FindMissingSkills(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords)
    {
        if (jdKeywords == null) return new List<string>();
    
        // If resume is null, all clean JD skills are missing
        if (resumeKeywords == null) 
        {
            return jdKeywords
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.Trim().ToLower()) // Keeps your lowercase choice
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