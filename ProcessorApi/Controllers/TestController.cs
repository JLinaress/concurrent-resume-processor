// diagnostic endpoint class that exposes DocumentParser service through REST API calls to verify PDF/Word 
// parsing works before building the full batch processor.
using Microsoft.AspNetCore.Mvc;
using ProcessorLib.Services;

namespace ProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]  // ← ADD: Base test
    public IActionResult Get() => Ok(new { message = "alive!", time = DateTime.UtcNow });
    
    [HttpGet("parse-pdf/{filePath}")]
    public IActionResult ParsePdf(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            Console.WriteLine($"Looking for: {fullPath}");  // Terminal log
            if (!System.IO.File.Exists(fullPath))
                return BadRequest($"File not found: {fullPath}");
        
            var text = DocumentParser.ParsePdf(fullPath);
            return Ok(new { chars = text.Length, preview = text[..Math.Min(200, text.Length)], path = fullPath });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");  // Terminal
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("parse-word/{filePath}")]
    public IActionResult ParseWord(string filePath) => Ok(DocumentParser.ParseWord(filePath));
}