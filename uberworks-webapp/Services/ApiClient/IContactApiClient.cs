// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/contact endpoints (both
//               suggest-service and the general "Contáctanos" message). HomeController.cs
//               depends on this interface, not on ContactApiClient.cs directly. Attachment/
//               image are plain ASP.NET Core IFormFile — the same object
//               HomeController.SuggestService/Contact receive straight from the browser's
//               upload, passed through untouched.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using Microsoft.AspNetCore.Http;

namespace uberworks_webapp.Services.ApiClient;

public interface IContactApiClient
{
    Task SuggestServiceAsync(string name, bool isFromCompany, string? companyName, string email, string message, IFormFile? attachment);

    /// <summary>Backs the general "Contáctanos" page (footer link), distinct from SuggestServiceAsync.</summary>
    Task SendMessageAsync(string title, string message, string name, string email, bool isFromCompany, string? companyName, IFormFile? image);
}
