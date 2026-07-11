using ProcessorLib.Models;

namespace ProcessorLib.Contracts;

public interface IMatchProcessorService
{
    Task<MatchResult> ProcessAsync(string resumeText, string jdText, CancellationToken token);
}