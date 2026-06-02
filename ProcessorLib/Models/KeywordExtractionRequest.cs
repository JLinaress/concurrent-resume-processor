using System.ComponentModel.DataAnnotations;

namespace ProcessorLib.Models;

public class KeywordExtractionRequest
{
    [Required]
    [MaxLength(100)]
    public string Text { get; set; } = string.Empty;
}