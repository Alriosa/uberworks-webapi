// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/contact/message — the general
//               "Contáctanos" page linked from the site footer (_LandingFooter.cshtml),
//               distinct from ServiceSuggestionRequest.cs ("didn't find the service you
//               needed?" on AllServices.cshtml specifically). Sent as multipart/form-data
//               (Image), same [FromForm] binding reasoning. No authentication required —
//               anyone browsing the public site can use this.
// Entities connected: None directly — ContactService.cs turns this into an email, it never
//                      touches the database
// Tables related: None directly (though ContactService.cs does log the submission to
//                 TBL_USER_ACTION_LOGS via IAuditLogService)
// =====================================================================================
using Microsoft.AspNetCore.Http;

namespace uberworks_webapi.Models.DTOs.Requests;

public class ContactUsRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsFromCompany { get; set; }
    public string? CompanyName { get; set; }
    public IFormFile? Image { get; set; }
}
