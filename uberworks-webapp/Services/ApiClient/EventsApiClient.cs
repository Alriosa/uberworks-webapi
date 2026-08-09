// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IEventsApiClient.cs. Same typed-HttpClient
//               pattern as every other client in this project — the Bearer token goes on
//               the individual HttpRequestMessage, never on _httpClient.DefaultRequestHeaders.
// Entities connected: None — this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class EventsApiClient : IEventsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public EventsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EventResponse> CreateAsync(string accessToken, CreateEventRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/events")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<EventResponse>(JsonOptions))!;
    }

    public async Task<List<EventResponse>> GetMyEventsAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/events/mine");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<EventResponse>>(JsonOptions))!;
    }

    public async Task<List<EventInvitationResponse>> GetMyInvitationsAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/events/invitations/mine");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<EventInvitationResponse>>(JsonOptions))!;
    }

    public async Task<EventInvitationResponse> RespondToInvitationAsync(string accessToken, int invitationId, bool accept)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/events/invitations/{invitationId}/respond")
        {
            Content = JsonContent.Create(new RespondToInvitationRequest { Accept = accept }, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<EventInvitationResponse>(JsonOptions))!;
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
