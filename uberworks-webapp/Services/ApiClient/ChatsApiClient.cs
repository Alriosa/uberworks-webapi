// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IChatsApiClient.cs. Same typed-HttpClient
//               pattern as every other client in this project — the Bearer token goes on the
//               individual HttpRequestMessage, never on _httpClient.DefaultRequestHeaders.
// Entities connected: None — this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class ChatsApiClient : IChatsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ChatsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ChatMessageResponse>> GetConversationAsync(string accessToken, int serviceId)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/chats/by-service/{serviceId}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<ChatMessageResponse>>(JsonOptions))!;
    }

    public async Task<ChatMessageResponse> SendMessageAsync(string accessToken, int serviceId, string message)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/chats/by-service/{serviceId}")
        {
            Content = JsonContent.Create(new SendChatMessageRequest { Message = message }, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ChatMessageResponse>(JsonOptions))!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
        throw new ApiException(response.StatusCode, error?.Message ?? "The request to the API failed.");
    }
}
