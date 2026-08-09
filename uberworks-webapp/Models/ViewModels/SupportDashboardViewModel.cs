// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Support.cshtml. Reports is the full REAL list from
//               GET /api/reports — the view itself splits it into the 3 status buckets
//               ("en vivo"=Open, "en espera/no resueltos"=Pending, "terminados"=Resolved) in
//               Razor rather than doing 3 separate API calls, since the whole list is small
//               enough for an internal support tool. Cancelled reports are deliberately
//               excluded from all 3 buckets (see ReportStatus.cs).
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_REPORTS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class SupportDashboardViewModel : DashboardViewModelBase
{
    public List<ReportResponse> Reports { get; set; } = new();
}
