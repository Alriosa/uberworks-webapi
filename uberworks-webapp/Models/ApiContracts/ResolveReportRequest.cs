// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/ResolveReportRequest.cs —
//               the body POST /api/reports/{id}/resolve expects. Backs the Support
//               dashboard's "Resolver" modal.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ResolveReportRequest
{
    public string Message { get; set; } = string.Empty;
    public ReportPaymentOutcome PaymentOutcome { get; set; }
}
