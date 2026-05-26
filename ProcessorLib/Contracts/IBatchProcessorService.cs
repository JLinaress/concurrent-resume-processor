using ProcessorLib.Models;

namespace ProcessorLib.Contracts;

public interface IBatchProcessorService
{
    Task<List<MatchResult>> ProcessBatchAsync(string resumeText, List<string> jdTexts, CancellationToken token);
}