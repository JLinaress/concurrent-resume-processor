namespace ProcessorLib.Models;

public class MatchResult
{
    public double Score { get; set; }

    public List<string> JdKeywords { get; set; } = [];

    public List<string> MissingSkills { get; set; } = [];
    
    public string TailoredResumeMarkdown { get; set; } = "";

    public string Recommendation => Score >= 70 ? "Apply!" : Score >= 50 ? "Tailor!" : "Move on";
}