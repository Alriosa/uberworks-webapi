// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IReportsApiClient.cs. CreateAsync builds a
//               MultipartFormDataContent (same reasoning as ContactApiClient.cs) since the
//               "puede tener imágenes" section needs multipart/form-data, not JSON.
//               UpdateAsync/CancelAsync are plain JSON like every other typed client.
// Entities connected: None — this only talks to the API over HTTP
// Tables related: None
// =====================================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public class ReportsApiClient : IReportsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ReportsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReportResponse> CreateAsync(
        string accessToken,
        string title,
        string description,
        int? serviceId,
        int? clientUserId,
        int? professionalUserId,
        DateTime? incidentDate,
        List<IFormFile>? images)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(title), "Title" },
            { new StringContent(description), "Description" }
        };

        if (serviceId is int svc)
        {
            content.Add(new StringContent(svc.ToString()), "ServiceId");
        }

        if (clientUserId is int client)
        {
            content.Add(new StringContent(client.ToString()), "ClientUserId");
        }

        if (professionalUserId is int professional)
        {
            content.Add(new StringContent(professional.ToString()), "ProfessionalUserId");
        }

        if (incidentDate is DateTime incident)
        {
            content.Add(new StringContent(incident.ToString("O")), "IncidentDate");
        }

        // Streams are only read during SendAsync below; disposing `content` at the end of
        // this method (via the outer `using`) disposes each StreamContent along with it.
        if (images is not null)
        {
            foreach (var image in images)
            {
                if (image.Length == 0)
                {
                    continue;
                }

                var fileContent = new StreamContent(image.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                content.Add(fileContent, "Images", image.FileName);
            }
        }

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/reports") { Content = content };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
    }

    public async Task<ReportResponse> ContactSupportAsync(string accessToken, string title, string description, int? serviceId, List<IFormFile>? images)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(title), "Title" },
            { new StringContent(description), "Description" }
        };

        if (serviceId is int svc)
        {
            content.Add(new StringContent(svc.ToString()), "ServiceId");
        }

        // Streams are only read during SendAsync below; disposing `content` at the end of
        // this method (via the outer `using`) disposes each StreamContent along with it.
        if (images is not null)
        {
            foreach (var image in images)
            {
                if (image.Length == 0)
                {
                    continue;
                }

                var fileContent = new StreamContent(image.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                content.Add(fileContent, "Images", image.FileName);
            }
        }

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/reports/contact-support") { Content = content };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
    }

    public async Task<List<ReportResponse>> GetAllAsync(string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, "api/reports");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<List<ReportResponse>>(JsonOptions))!;
    }

    public async Task<ReportResponse> GetByIdAsync(string accessToken, int id)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/reports/{id}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
    }

    public async Task<ReportResponse> ResolveAsync(string accessToken, int id, ResolveReportRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/reports/{id}/resolve")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
    }

    public async Task<ReportResponse> NoFaultAsync(string accessToken, int id)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/reports/{id}/no-fault");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
    }

    public async Task<ReportResponse> UpdateAsync(string accessToken, int id, UpdateReportRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"api/reports/{id}")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
    }

    public async Task<ReportResponse> CancelAsync(string accessToken, int id, CancelReportRequest request)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"api/reports/{id}/cancel")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        await EnsureSuccessAsync(response);

        return (await response.Content.ReadFromJsonAsync<ReportResponse>(JsonOptions))!;
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
