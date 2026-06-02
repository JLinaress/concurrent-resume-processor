using System.ComponentModel.DataAnnotations;

namespace ProcessorLib.Models;

public class BatchRequest
{
    [Required]
    [MaxLength(100)]
    public string ResumeContent { get; set; } = "";
    
    [Required]
    [MaxLength(100)]
    public List<string> JdContents { get; set; } = new();
}