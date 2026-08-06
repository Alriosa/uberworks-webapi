// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Company.cshtml. Adds WorkerCount, pulled from
//               GET /api/professionals/my-workers, on top of DashboardViewModelBase.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class CompanyDashboardViewModel : DashboardViewModelBase
{
    public int WorkerCount { get; set; }
}
