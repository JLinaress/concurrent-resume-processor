using System.Text.RegularExpressions;
using ProcessorLib.Contracts;

namespace ProcessorLib.Services;

public class KeywordExtractor : IKeywordExtractor
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "the","a","an","and","or","but","in","on","at","to","for","of","with","by","is","are","was","were",
        "as","from","that","this","it","be","been","being","will","would","can","could","should","may","might",
        "you","your","we","our","they","their","i","me","my","he","she","his","her","them","than","then", "not", "has"
    };
    
    public IEnumerable<string> ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Enumerable.Empty<string>();
        
        var paddedText = Regex.Replace(text, @"(?<i>\bdotnet\b|\b\.net\b|\bcsharp\b|\bc\#\b)", " ${i} ", RegexOptions.IgnoreCase);
        
        var clean = CleanText(paddedText);
        
        return clean
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeToken)
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Where(w => ((w.Length >= 3 || 
                          w == "c#" ||
                          w.Contains('/') || 
                          w.StartsWith('.')   
                ) && !KeywordExtractor.StopWords.Contains(w)))  
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
    
    public string NormalizeToken(string token)
    {
        token = token.Trim().ToLower();

        if (token == ".net" || token == "dotnet") return ".net";
        if (token == "csharp" || token == "c#") return "c#";
        
        token = token.Trim('.', '/', '-', '|');

        if (token == "c#") return "c#";
        if (token == ".net") return ".net";

        return token.EndsWith('s') && token.All(char.IsLetter) && !token.EndsWith("ss") && !token.EndsWith("is") && token.Length > 3 
            ? token[..^1]
            : token;
    }
    
    private static string CleanText(string text) => 
        Regex.Replace(text ?? "", @"[^\w\s\-\|\#\/\.]", " ").ToLower();
}