using ProcessorLib.Models;
using ProcessorWebUI.Contracts;

namespace ProcessorWebUI.Services;

public class BatchMatchService : IBatchMatchService
{
    private readonly HttpClient _client;
    
    public BatchMatchService()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5000/");
    }
    
    public async Task<List<MatchResult>> MatchBatchAsync(BatchRequest request)
    {
        var response = await _client.PostAsJsonAsync("api/batch/match", request);
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<MatchResult>>() ?? new List<MatchResult>();
    }
}