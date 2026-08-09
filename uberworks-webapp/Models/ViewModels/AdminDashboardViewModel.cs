// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Admin.cshtml — which is now ALSO the MasterAdmin
//               dashboard (see DashboardController.LandingPage's UserRole.MasterAdmin case).
//               There used to be a separate MasterAdmin.cshtml/MasterAdminDashboardViewModel
//               with decorative (fake) Trabajos/Reportes panels; the user asked for full
//               parity ("el master admin tiene que ser capaz de hacer todo lo que los demás
//               pueden hacer"), so MasterAdmin now renders this exact same real-CRUD view,
//               with IsMasterAdmin=true adding the one extra "Finanzas" card MasterAdmin
//               alone gets (see Admin.cshtml). Every list/action here is REAL and round-trips
//               to the API: Users (GET /api/users), Services (GET /api/services, the
//               Admin-only every-status listing — not GetOpenAsync/GetMineAsync), Reports
//               (GET /api/reports). See DashboardController.LandingPage for how these are
//               loaded and the various UpdateXAdmin/DeleteXAdmin/CreateReport/CancelReportAdmin
//               actions for what each CRUD button actually does.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS/TBL_SERVICES/TBL_REPORTS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class AdminDashboardViewModel : DashboardViewModelBase
{
    public List<AdminUserListItemResponse> Users { get; set; } = new();
    public List<AdminServiceListItemResponse> Services { get; set; } = new();
    public List<ReportResponse> Reports { get; set; } = new();

    /// <summary>True only when the logged-in caller's Role is MasterAdmin — adds the
    /// "Finanzas" card and swaps the header title/icon, but every other panel on this
    /// page behaves identically for Admin and MasterAdmin.</summary>
    public bool IsMasterAdmin { get; set; }
}
