namespace ProcessorLib.Services;

public class MatchScorer
{
    public double CalculateMatchScore(HashSet<string> resumeKeywords, HashSet<string> jdKeywords)
    {
        if(jdKeywords.Count == 0) return 0.0;
        
        //filter out nulls silently
        var resumeFiltered = resumeKeywords.Where(k => k != null).ToHashSet();
        var jdFiltered = jdKeywords.Where(k => k != null).ToHashSet();
        
        if(jdFiltered.Count == 0) return 0.0;
        
        var intersectionCount = resumeFiltered.Intersect(jdKeywords, StringComparer.OrdinalIgnoreCase).Count();
        var unionCount = resumeFiltered.Union(jdKeywords, StringComparer.OrdinalIgnoreCase).Count();
        
        // Avoid division by zero
        if(unionCount == 0) return 0.0;
        
        // get overlap between the two sets
        var coverage = (double)intersectionCount / unionCount;
        
        // intersection over union  %
        return Math.Round(coverage * 100.0, 2);
    }

    public List<string> FindMissingSkills(HashSet<string> resumeKeywords, HashSet<string> jdKeywords) =>
        jdKeywords.Except(resumeKeywords, StringComparer.OrdinalIgnoreCase)
                  .Take(20)
                  .ToList();
    
    public List<string> FindStrongMatches(HashSet<string> resumeKeywords, HashSet<string> jdKeywords) =>
        resumeKeywords.Intersect(jdKeywords, StringComparer.OrdinalIgnoreCase)
                      .Take(20)
                      .ToList();
}