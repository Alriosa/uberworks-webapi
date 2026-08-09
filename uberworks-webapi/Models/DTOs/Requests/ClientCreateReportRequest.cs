// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/reports/contact-support — a Client's own
//               "Contactar con Soporte" self-service case, per explicit request (title, body
//               text, optional image upload, optional associated Service/case id). Bound as
//               [FromForm] for the same reason as CreateReportRequest.cs — Images travels
//               alongside it in the same multipart request. Unlike CreateReportRequest.cs
//               (Admin/Support filing on someone else's behalf, ClientUserId/ProfessionalUserId
//               both settable), this DTO has NO ClientUserId field at all — ReportService.
//               CreateFromClientAsync always uses the caller's own id, so a Client can never
//               file a case pretending to be someone else.
// Entities connected: Report.cs (indirectly, via ReportService.CreateFromClientAsync)
// Tables related: None directly (TBL_REPORTS is filled in from ReportService.cs)
// =====================================================================================
using Microsoft.AspNetCore.Http;

namespace uberworks_webapi.Models.DTOs.Requests;

public class ClientCreateReportRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Optional — the Service/job id this case is about, so Support can find it by
    /// the same id shown on the client's own job history. Must belong to the caller.</summary>
    public int? ServiceId { get; set; }

    public List<IFormFile>? Images { get; set; }
}
