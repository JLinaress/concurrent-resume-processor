using ProcessorLib.Models;

namespace ProcessorLib.Contracts;

public interface IBatchProcessorService
{
    MatchResult ProcessAsync(string resumeText, string jdText, CancellationToken token);
}