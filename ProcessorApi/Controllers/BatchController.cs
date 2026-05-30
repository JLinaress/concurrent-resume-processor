using Microsoft.AspNetCore.Mvc;
using ProcessorLib.Contracts;
using ProcessorLib.Models;

namespace ProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IBatchProcessorService _processor;
    
    public BatchController(IBatchProcessorService processor)
    {
        _processor = processor;
    }
    
    [HttpPost("match")]
    public async Task<ActionResult<List<MatchResult>>> MatchBatch([FromBody] BatchRequest request) =>
        Ok(await _processor.ProcessBatchAsync(request.ResumeContent, request.JdContents, CancellationToken.None));
}