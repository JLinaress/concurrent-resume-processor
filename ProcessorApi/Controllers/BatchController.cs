using Microsoft.AspNetCore.Mvc;
using ProcessorLib.Contracts;
using ProcessorLib.Models;

namespace ProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IBatchProcessorService _processor;
    private readonly IKeywordExtractor _extractor;
    
    public BatchController(IBatchProcessorService processor, IKeywordExtractor extractor)
    {
        _processor = processor;
        _extractor = extractor;
    }
    
    [HttpPost("match")]
    public async Task<ActionResult<List<MatchResult>>> MatchBatch([FromBody] BatchRequest request) =>
        Ok(await _processor.ProcessBatchAsync(request.ResumeContent, request.JdContents, CancellationToken.None));

    [HttpPost("extract")]
    public ActionResult<List<string>> ExtractKeywords([FromBody] KeywordExtractionRequest request ) =>
        Ok( _extractor.ExtractKeywords(request.Text).ToList());
}