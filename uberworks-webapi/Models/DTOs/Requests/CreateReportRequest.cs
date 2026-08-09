// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/reports — filing a new dispute report from
//               either the Admin or Support dashboard. Bound as [FromForm] (not JSON) because
//               Images travels alongside it in the same multipart request — same reasoning
//               as ProfessionalsController.UploadPhoto for why file uploads can't share a
//               plain JSON body. ServiceId/ClientUserId/ProfessionalUserId are all optional:
//               there is no "flag this job" flow yet on the Client/Professional side (see
//               Report.cs's FILE SUMMARY), so whoever files it fills in whatever is known.
// Entities connected: Report.cs (indirectly, via ReportService.CreateAsync)
// Tables related: None directly (TBL_REPORTS is filled in from ReportService.cs)
// =====================================================================================
using Microsoft.AspNetCore.Http;

namespace uberworks_webapi.Models.DTOs.Requests;

public class CreateReportRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ServiceId { get; set; }
    public int? ClientUserId { get; set; }
    public int? ProfessionalUserId { get; set; }
    public DateTime? IncidentDate { get; set; }

    /// <summary>Optional — the "puede tener imágenes si deciden subirlas" section.</summary>
    public List<IFormFile>? Images { get; set; }
}
