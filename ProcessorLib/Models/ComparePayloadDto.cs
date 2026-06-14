using System.ComponentModel.DataAnnotations;

namespace ProcessorLib.Models;

public class ComparePayloadDto
{
    [StringLength(int.MaxValue)]
    public string ResumeContent { get; set; } = "";
    
    [StringLength(int.MaxValue)]
    public string JdContent { get; set; } = "";
}