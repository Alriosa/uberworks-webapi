// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/reports/{id}/cancel — used both by the
// Admin dashboard's report CRUD "Borrar" action and the Support dashboard's "Cancelar
// reporte" button (the same underlying operation: see ReportService.CancelAsync). Reason is
// required — the user explicitly asked that cancelling always demands an explanation.
// Entities connected: Report.cs (indirectly, via ReportService.CancelAsync)
// Tables related: None directly (TBL_REPORTS is updated from ReportService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CancelReportRequest
{
    public string Reason { get; set; } = string.Empty;
}
