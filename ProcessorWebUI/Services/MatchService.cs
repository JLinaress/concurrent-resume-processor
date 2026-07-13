using ProcessorLib.Models;
using ProcessorWebUI.Contracts;

namespace ProcessorWebUI.Services;

public class MatchService : IMatchService
{
    private readonly HttpClient _client;
    
    public MatchService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("ProcessorApi");
    }
    
    public async Task<MatchResult> MatchAsync(MatchRequest request,  CancellationToken token)
    {
        var response = await _client.PostAsJsonAsync("api/match", request, token);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<MatchResult>(cancellationToken: token) 
               ?? throw new InvalidOperationException("Failed to deserialize MatchResult");
    }
}