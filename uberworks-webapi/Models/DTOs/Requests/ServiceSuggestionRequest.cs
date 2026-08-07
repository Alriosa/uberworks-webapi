// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/contact/suggest-service — the "didn't find
//               the service you needed?" form on the WebApp's AllServices page. Sent as
//               multipart/form-data (not JSON) because of Attachment, so this binds via
//               [FromForm] in ContactController.cs, not [FromBody] like every other request
//               DTO in this folder. No authentication required to submit this — anyone
//               browsing the public site, logged in or not, can suggest a service.
// Entities connected: None directly — ContactService.cs turns this into an email, it never
//                      touches the database
// Tables related: None directly (though ContactService.cs does log the submission to
//                 TBL_USER_ACTION_LOGS via IAuditLogService)
// =====================================================================================
using Microsoft.AspNetCore.Http;

namespace uberworks_webapi.Models.DTOs.Requests;

public class ServiceSuggestionRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsFromCompany { get; set; }
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IFormFile? Attachment { get; set; }
}
