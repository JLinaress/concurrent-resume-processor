using Microsoft.AspNetCore.Mvc;
using ProcessorLib.Contracts;
using ProcessorLib.Models;

namespace ProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly IMatchProcessorService _processor;
    private readonly IKeywordExtractor _extractor;
    
    public MatchController(IMatchProcessorService processor, IKeywordExtractor extractor)
    {
        _processor = processor;
        _extractor = extractor;
    }
    
    [HttpPost("match")]
    public  ActionResult<MatchResult> Match([FromBody] MatchRequest request) =>
        Ok(_processor.ProcessAsync(request.ResumeContent, request.JdContent, CancellationToken.None));
    
    [HttpPost("extract")]
    public ActionResult<List<string>> ExtractKeywords([FromBody] KeywordExtractionRequest request ) =>
        Ok( _extractor.ExtractKeywords(request.Text).ToList());

    [HttpPost("compare")]
    public ActionResult Compare([FromBody] ComparePayloadDto payload)
    {
        if (payload == null) return BadRequest("Request cannot be null");
        
        // Extract unique text from both blocks
        var resume = _extractor.ExtractKeywords(payload.ResumeContent).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var jd = _extractor.ExtractKeywords(payload.JdContent).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        // Calculate matches & gaps
        var matchKeywords = resume.Intersect(jd, StringComparer.OrdinalIgnoreCase).ToList();
        var missingKeywords = jd.Except(resume, StringComparer.OrdinalIgnoreCase).ToList();
        
        // Return clean analysis 
        return Ok(new
        {
            Matches = matchKeywords,
            MissingKeywords = missingKeywords,
            MatchCount = matchKeywords.Count,
            MissingCount = matchKeywords.Count
        });
    }
}