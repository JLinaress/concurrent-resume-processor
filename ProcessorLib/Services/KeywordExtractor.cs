using System.Text.RegularExpressions;
using ProcessorLib.Contracts;

namespace ProcessorLib.Services;

public class KeywordExtractor : IKeywordExtractor
{
    private static readonly HashSet<string> TechKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "c#", ".net", "asp.net", "javascript", "typescript", "react", "vue", "vue.js",
        "java", "python", "sql", "sql server", "oracle", "mysql", "postgresql", "redis",
        "kafka", "docker", "kubernetes", "azure", "aws", "gitlab", "git", "ci/cd",
        "helm", "argo", "gitops", "microservices", "graphql", "rest", "api",
        "linux", "windows", "bash", "shell", "terraform", "ansible", "jenkins",
        "c++", "scala", "go", "rust", "ruby", "php", "kotlin", "html", 
        "css",  "ada", "fortran", "assembly", "xml", "asp", "access", "mongodb",
        "rabbitmq", "full-stack", "unix", "solaris", "tcp/ip", "dotnet", "node.js", "f#"
    };
    
    public IEnumerable<string> ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Enumerable.Empty<string>();
    
        var clean = CleanText(text);
    
        Console.WriteLine($"EXTRACTOR DEBUG: Input length = {text.Length}");
    
        // ONLY extract known tech keywords (no regular tokens)
        var techKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tech in TechKeywords)
        {
            if (clean.Contains(tech, StringComparison.OrdinalIgnoreCase))
            {
                techKeywords.Add(tech);
            }
        }
    
        return techKeywords; 
    }
    
    public string NormalizeToken(string token)
    {
        token = token.Trim().ToLower();

        if (token == "dotnet" || token == ".net") return ".net";
        if (token == "csharp" || token == "c#") return "c#";
        if (token == "nodejs" || token == "node.js") return "node.js";
        if (token == "python" || token == "python") return "python";
        if (token == "typescript" || token == "ts") return "typescript";
        
        token = token.Trim('.', '/', '-', '|');
        
        return token;
    }
    
    private static string CleanText(string text) => 
        Regex.Replace(text ?? "", @"[^\w\s\-\|\#\/\.]", " ").ToLower();
}