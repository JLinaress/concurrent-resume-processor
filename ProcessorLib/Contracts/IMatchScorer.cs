namespace ProcessorLib.Contracts;

public interface IMatchScorer
{
    double CalculateMatchScore(IEnumerable<string> resumeKeywords, IEnumerable<string> jdKeywords);
    
    List<string> FindMissingSkills(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords);
    
    List<string> FindStrongMatches(IEnumerable<string>? resumeKeywords, IEnumerable<string>? jdKeywords);
}