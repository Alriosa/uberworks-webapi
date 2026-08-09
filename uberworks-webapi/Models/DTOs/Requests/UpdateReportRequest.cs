// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of PUT /api/reports/{id} — editing a report's own facts
//               (title/description/parties/incident date) from the Admin dashboard's CRUD
//               panel. Deliberately does NOT include Status/ResolutionMessage/
//               PaymentOutcome/CancellationReason — those only change through the dedicated
//               Resolve/NoFault/Cancel actions (see IReportService), which also log who did
//               it and when, unlike a plain field edit.
// Entities connected: Report.cs (indirectly, via ReportService.UpdateAsync)
// Tables related: None directly (TBL_REPORTS is updated from ReportService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateReportRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ServiceId { get; set; }
    public int? ClientUserId { get; set; }
    public int? ProfessionalUserId { get; set; }
    public DateTime? IncidentDate { get; set; }
}
