using System.ComponentModel.DataAnnotations;

namespace ProcessorLib.Models;

public class BatchRequest
{
    [StringLength(int.MaxValue)]
    public string ResumeContent { get; set; } = "";
    
    public List<string> JdContents { get; set; } = new();
}