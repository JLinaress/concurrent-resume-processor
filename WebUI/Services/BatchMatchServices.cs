// using System.Net.Http.Json;
// using ProcessorLib.Models;
// using ProcessorWebUI.Contracts;
//
// namespace ProcessorWebUI.Services;
//
// public class BatchMatchService : IBatchMatchService
// {
//     private readonly HttpClient _client;
//     
//     public BatchMatchService(IHttpClientFactory clientFactory)
//     {
//         _client = clientFactory.CreateClient("ProcessorApi");
//     }
//
//     public async Task<List<MatchResult>> MatchBatchAsync(BatchRequest request)
//     {
//         var response = await _client.PostAsJsonAsync("api/batch/match", request);
//         
//         response.EnsureSuccessStatusCode();
//
//         return await response.Content.ReadFromJsonAsync<List<MatchResult>>() ?? new List<MatchResult>();
//     }
// }