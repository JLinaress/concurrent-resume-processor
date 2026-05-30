using ProcessorLib.Models;

namespace ProcessorWebUI.Contracts;

public interface IBatchMatchService
{
    Task<List<MatchResult>> MatchBatchAsync(BatchRequest request);
}