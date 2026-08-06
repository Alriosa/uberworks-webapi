// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IProfessionalsApiClient.cs. Uses the same typed
//               HttpClient as UsersApiClient.cs (registered in Program.cs with the API's
//               base URL and "X-Client-Source"/"X-Internal-Secret" headers already
//               attached), but adds "Authorization: Bearer {accessToken}" on each individual
//               HttpRequestMessage — never on _httpClient.DefaultRequestHeaders, which is
//               shared across every request the typed client makes — so one Company's token
//               never leaks into a request made on behalf of a different logged-in user.
// Entities connected: None — this project has no database entities; this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class ProfessionalsApiClient : IProfessionalsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ProfessionalsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProfessionalResponse> CreateWorkerAsync(string accessToken, CompanyCreateWorkerRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/professionals/company-create")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ProfessionalResponse>(JsonOptions))!;
    }

    public async Task<List<ProfessionalResponse>> GetMyWorkersAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/professionals/my-workers");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<ProfessionalResponse>>(JsonOptions))!;
    }

    public async Task<ProfessionalResponse> GetByUserIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"api/professionals/by-user/{userId}");
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ProfessionalResponse>(JsonOptions))!;
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
