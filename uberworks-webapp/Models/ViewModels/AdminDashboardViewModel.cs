// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Admin.cshtml. Adds nothing to DashboardViewModelBase
//               yet — kept as its own class/view (not shared with Manager/MasterAdmin) so
//               this is the one place to extend later without touching anyone else's
//               dashboard.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class AdminDashboardViewModel : DashboardViewModelBase
{
}
