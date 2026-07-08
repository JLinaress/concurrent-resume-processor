using ProcessorLib.Models;

namespace ProcessorWebUI.Contracts;

public interface IBatchMatchService
{
    Task<MatchResult> MatchAsync(BatchRequest request);
}