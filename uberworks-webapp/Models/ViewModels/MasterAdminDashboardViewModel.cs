// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/MasterAdmin.cshtml. Adds nothing to
//               DashboardViewModelBase yet — there's only ever one MasterAdmin account.
//               Kept as its own class/view so this is the one place to extend later
//               without touching anyone else's dashboard.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class MasterAdminDashboardViewModel : DashboardViewModelBase
{
}
