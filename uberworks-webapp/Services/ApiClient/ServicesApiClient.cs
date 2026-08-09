// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IServicesApiClient.cs. Same typed-HttpClient
//               pattern as ProfessionalsApiClient.cs — the Bearer token (when needed) goes
//               on the individual HttpRequestMessage, never on _httpClient.DefaultRequestHeaders.
// Entities connected: None — this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class ServicesApiClient : IServicesApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ServicesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ServiceResponse>> GetOpenAsync()
    {
        var response = await _httpClient.GetAsync("api/services/open");
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<ServiceResponse>>(JsonOptions))!;
    }

    public async Task<List<ServiceResponse>> GetMineAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/services/mine");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<ServiceResponse>>(JsonOptions))!;
    }

    public async Task<List<AdminServiceListItemResponse>> GetAllForAdminAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/services");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<AdminServiceListItemResponse>>(JsonOptions))!;
    }

    public async Task<ServiceResponse> UpdateForAdminAsync(string accessToken, int id, UpdateServiceAdminRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/services/{id}")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ServiceResponse>(JsonOptions))!;
    }

    public async Task DeleteForAdminAsync(string accessToken, int id)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/services/{id}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);
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
