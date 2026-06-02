using System.ComponentModel.DataAnnotations;

namespace ProcessorLib.Models;

public class ComparePayloadDto
{
    [Required]
    [MaxLength(100000, ErrorMessage = "Text cannot exceed 100,000 characters.")]

    public string ResumeContent { get; set; } = String.Empty;
    
    [Required]
    [MaxLength(100000)]

    public string JdContent { get; set; } = "";
}