// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IContactApiClient.cs. Unlike every other API
//               client in this project (all plain JSON via PostAsJsonAsync), this one builds
//               a MultipartFormDataContent — the file upload means the request has to be
//               multipart/form-data, not JSON, matching what ContactController.cs on the API
//               side expects via [FromForm].
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

public class ContactApiClient : IContactApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ContactApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SuggestServiceAsync(string name, bool isFromCompany, string? companyName, string email, string message, IFormFile? attachment)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(name), "Name" },
            { new StringContent(isFromCompany.ToString()), "IsFromCompany" },
            { new StringContent(email), "Email" },
            { new StringContent(message), "Message" }
        };

        if (!string.IsNullOrWhiteSpace(companyName))
        {
            content.Add(new StringContent(companyName), "CompanyName");
        }

        // The stream is only read during SendAsync below; disposing `content` at the end of
        // this method (via the outer `using`) disposes this StreamContent along with it.
        if (attachment is not null)
        {
            var fileContent = new StreamContent(attachment.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(attachment.ContentType);
            content.Add(fileContent, "Attachment", attachment.FileName);
        }

        var response = await _httpClient.PostAsync("api/contact/suggest-service", content);
        await EnsureSuccessAsync(response);
    }

    public async Task SendMessageAsync(string title, string message, string name, string email, bool isFromCompany, string? companyName, IFormFile? image)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(title), "Title" },
            { new StringContent(message), "Message" },
            { new StringContent(name), "Name" },
            { new StringContent(email), "Email" },
            { new StringContent(isFromCompany.ToString()), "IsFromCompany" }
        };

        if (!string.IsNullOrWhiteSpace(companyName))
        {
            content.Add(new StringContent(companyName), "CompanyName");
        }

        // The stream is only read during SendAsync below; disposing `content` at the end of
        // this method (via the outer `using`) disposes this StreamContent along with it.
        if (image is not null)
        {
            var fileContent = new StreamContent(image.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
            content.Add(fileContent, "Image", image.FileName);
        }

        var response = await _httpClient.PostAsync("api/contact/message", content);
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
