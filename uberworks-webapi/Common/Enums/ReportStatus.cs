// =====================================================================================
// FILE SUMMARY
// What it does: Defines what stage a dispute/incident Report is in — this is what the
//               Support dashboard's 3-way split ("reportes en vivo", "en espera/no
//               resueltos", "terminados") is grounded on: Open -> "en vivo" (just filed,
//               needs attention), Pending -> "en espera/no resueltos" (Support looked at it
//               but it's stuck waiting on something), Resolved -> "terminados" (closed via
//               either the "Resolver" or "Fallo a favor de nadie" action — see
//               ReportPaymentOutcome.cs for how those two are told apart). Cancelled reports
//               are deliberately excluded from all 3 buckets — see ReportService.CancelAsync.
// Entities connected: Report.cs (the Report.Status property is of this type)
// Tables related: TBL_REPORTS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the CHECK constraint on TBL_REPORTS.CL_STATUS.
/// </summary>
public enum ReportStatus
{
    Open,
    Pending,
    Resolved,
    Cancelled
}
