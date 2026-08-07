// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/contact endpoint.
//               HomeController.cs depends on this interface, not on ContactApiClient.cs
//               directly. Attachment is a plain ASP.NET Core IFormFile — the same object
//               HomeController.SuggestService receives straight from the browser's upload,
//               passed through untouched.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using Microsoft.AspNetCore.Http;

namespace uberworks_webapp.Services.ApiClient;

public interface IContactApiClient
{
    Task SuggestServiceAsync(string name, bool isFromCompany, string? companyName, string email, string message, IFormFile? attachment);
}
