using ProcessorLib.Models;

namespace ProcessorLib.Contracts;

public interface IBatchProcessorService
{
    Task<List<MatchResult>> ProcessBatchAsync(string resumeText, IEnumerable<string> jdTexts, CancellationToken token);
}