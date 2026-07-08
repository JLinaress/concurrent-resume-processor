using ProcessorLib.Models;
using ProcessorWebUI.Contracts;

namespace ProcessorWebUI.Services;

public class BatchMatchService : IBatchMatchService
{
    private readonly HttpClient _client;
    
    public BatchMatchService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("ProcessorApi");
    }
    
    public async Task<MatchResult> MatchAsync(BatchRequest request)
    {
        var response = await _client.PostAsJsonAsync("api/batch/match", request);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<MatchResult>() 
               ?? throw new InvalidOperationException("Failed to deserialize MatchResult");
    }
}