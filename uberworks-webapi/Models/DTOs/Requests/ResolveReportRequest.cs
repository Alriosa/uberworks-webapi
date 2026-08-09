// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/reports/{id}/resolve — the Support
// dashboard's "Resolver" button. Message is what gets recorded as the report's resolution
// (see ReportService.ResolveAsync for the honest note on why it's stored on the report
// itself rather than actually delivered into a live two-way chat inbox — Chat.cs has no
// Repository/Service/Controller yet, see Task #70). PaymentOutcome is mandatory here:
// resolving always requires deciding whether the held payment goes to the professional or
// back to the client.
// Entities connected: Report.cs (indirectly, via ReportService.ResolveAsync)
// Tables related: None directly (TBL_REPORTS is updated from ReportService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class ResolveReportRequest
{
    public string Message { get; set; } = string.Empty;
    public ReportPaymentOutcome PaymentOutcome { get; set; }
}
