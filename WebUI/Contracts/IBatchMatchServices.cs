using ProcessorLib.Models;

namespace WebUI.Contracts;

public interface IBatchMatchServices
{
        Task<List<MatchResult>> MatchBatchAsync(BatchRequest request);
}