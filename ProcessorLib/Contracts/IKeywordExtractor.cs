namespace ProcessorLib.Contracts;

public interface IKeywordExtractor
{
    IEnumerable<string> ExtractKeywords(string text);
    
    string NormalizeToken(string text);
}