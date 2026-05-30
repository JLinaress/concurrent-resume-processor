namespace ProcessorLib.Models;

public class BatchRequest
{
    public string ResumeContent { get; set; } = "";
    
    public List<string> JdContents { get; set; } = new();
}