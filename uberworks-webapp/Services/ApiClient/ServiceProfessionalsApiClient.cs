// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IServiceProfessionalsApiClient.cs. Uses a typed
//               HttpClient (registered in Program.cs) to call
//               POST/GET /api/services/{serviceId}/proposals and
//               POST /api/services/{serviceId}/proposals/{proposalId}/accept. Every call here
//               requires [Authorize] on the API side, so the caller's own JWT is attached as
//               "Authorization: Bearer {accessToken}" on that one HttpRequestMessage only —
//               same reasoning as UsersApiClient.cs. If the API responds with an error status,
//               throws Common/Exceptions/ApiException.cs with the API's real error message.
// Entities connected: None — WebApp has no database entities; this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class ServiceProfessionalsApiClient : IServiceProfessionalsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ServiceProfessionalsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ServiceProfessionalResponse> CreateProposalAsync(string accessToken, int serviceId, CreateServiceProfessionalRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/services/{serviceId}/proposals")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ServiceProfessionalResponse>(JsonOptions))!;
    }

    public async Task<List<ServiceProfessionalResponse>> GetProposalsAsync(string accessToken, int serviceId)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/services/{serviceId}/proposals");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<ServiceProfessionalResponse>>(JsonOptions))!;
    }

    public async Task<ServiceProfessionalResponse> AcceptProposalAsync(string accessToken, int serviceId, int proposalId)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/services/{serviceId}/proposals/{proposalId}/accept");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ServiceProfessionalResponse>(JsonOptions))!;
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
