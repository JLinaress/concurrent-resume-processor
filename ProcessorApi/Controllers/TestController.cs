// diagnostic endpoint class that exposes DocumentParser service through REST API calls to verify PDF/Word 
// parsing works before building the full batch processor.
using Microsoft.AspNetCore.Mvc;
using ProcessorApi.DTOs;
using ProcessorLib.Services;

//TODO remove this class and the TestDTO when you publish as it isn't needed and it is a vulnerability 
namespace ProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "We're Alive!", time = DateTime.UtcNow });
    
    [HttpPost] // ← ADD: Base test. This method is designed to accept enclosed payload 
    public IActionResult Post([FromBody] TestRequestDto request) => Ok(new { message = request.Message, time = request.Time });
    
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