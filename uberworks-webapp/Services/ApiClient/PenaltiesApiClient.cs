// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IPenaltiesApiClient.cs. Uses a typed HttpClient
//               (registered in Program.cs) to call GET /api/penalties/mine, attaching the
//               caller's own JWT as "Authorization: Bearer {accessToken}" on that one
//               HttpRequestMessage only — same reasoning as UsersApiClient.cs. If the API
//               responds with an error status, throws Common/Exceptions/ApiException.cs with
//               the API's real error message.
// Entities connected: None — WebApp has no database entities; this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class PenaltiesApiClient : IPenaltiesApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public PenaltiesApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PenaltyResponse>> GetMineAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/penalties/mine");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<PenaltyResponse>>(JsonOptions))!;
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
