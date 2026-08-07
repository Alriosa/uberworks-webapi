// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the "suggest a service" contact form's business logic.
//               ContactController.cs depends on this interface, not on ContactService.cs
//               directly.
// Entities connected: None
// Tables related: None
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;

namespace uberworks_webapi.Services.Interfaces;

public interface IContactService
{
    Task SuggestServiceAsync(ServiceSuggestionRequest request);
}
