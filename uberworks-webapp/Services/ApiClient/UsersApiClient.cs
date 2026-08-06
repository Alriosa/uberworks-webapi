// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IUsersApiClient.cs. Uses a typed HttpClient
//               (registered in Program.cs with the API's base URL and the
//               "X-Client-Source: WebApp" header already attached — see
//               uberworks-webapi's ICurrentUserService.Source, which reads that exact
//               header for audit logging) to call POST /api/users/login,
//               POST /api/users/register, and POST /api/users/admin-create. AdminCreateUserAsync
//               attaches "Authorization: Bearer {accessToken}" on that one HttpRequestMessage
//               only (never on _httpClient.DefaultRequestHeaders, which is shared across every
//               request the typed client makes) so it doesn't leak one admin's token into
//               unrelated login/register calls made by other users at the same time. If the
//               API responds with an error status, throws Common/Exceptions/ApiException.cs
//               with the API's real error message instead of a generic .NET HTTP exception.
// Entities connected: None — WebApp has no database entities; this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class UsersApiClient : IUsersApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public UsersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/login", request, JsonOptions);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions))!;
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/register", request, JsonOptions);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions))!;
    }

    public async Task<UserResponse> AdminCreateUserAsync(string accessToken, AdminCreateUserRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/users/admin-create")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions))!;
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
