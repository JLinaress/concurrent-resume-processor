using System.Text.RegularExpressions;

namespace ProcessorLib.Services;

public class KeywordExtractor
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "the","a","an","and","or","but","in","on","at","to","for","of","with","by","is","are","was","were",
        "as","from","that","this","it","be","been","being","will","would","can","could","should","may","might",
        "you","your","we","our","they","their","i","me","my","he","she","his","her","them","than","then", "not"
    };
    
    public HashSet<string> ExtractKeywords(string text)
    {
        var clean = Regex.Replace(text ?? "", @"[^\w\s\-\|\#\/\.]", " ").ToLower();
        
        return clean
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(NormalizeToken)
            .Where(w => ((w.Length >= 3 || 
                          w == "c#" ||
                          w.Contains('/') || // Allows CI/CD, TCP/IP
                          w.StartsWith('.')   // Allows .net, etc.
                          ) && !KeywordExtractor.StopWords.Contains(w)))  // Filter short words
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
    
    private static string NormalizeToken(string token)
    {
        token = token.Trim().ToLower();

        return token switch
        {
            ".net" => ".net",
            "dotnet" => ".net",
            "csharp" => "c#",
            "c#" => "c#",
            // Only strip 's' if the word contains only letters. 
            // This automatically protects node.js, c#, and ci/cd.
            _ => token.EndsWith('s') && token.All(char.IsLetter) && !token.EndsWith("ss") && !token.EndsWith("is") && token.Length > 3 
                ? token[..^1]
                : token
        };
    }
}