using ProcessorLib.Models;

namespace ProcessorWebUI.Contracts;

public interface IMatchService
{
    Task<MatchResult> MatchAsync(MatchRequest request);
}