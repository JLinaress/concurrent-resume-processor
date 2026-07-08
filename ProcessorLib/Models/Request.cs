using System.ComponentModel.DataAnnotations;

namespace ProcessorLib.Models;

public class BatchRequest
{
    [StringLength(int.MaxValue)]
    public string ResumeContent { get; set; } = "";
    
    [StringLength(int.MaxValue)]
    public string JdContent { get; set; } = "";
}