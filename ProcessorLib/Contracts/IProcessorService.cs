using ProcessorLib.Models;

namespace ProcessorLib.Contracts;

public interface IMatchProcessorService
{
    MatchResult ProcessAsync(string resumeText, string jdText, CancellationToken token);
}