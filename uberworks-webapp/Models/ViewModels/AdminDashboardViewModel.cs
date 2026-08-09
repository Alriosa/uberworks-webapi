// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Admin.cshtml. Unlike MasterAdmin.cshtml's "Ver Todos
//               los Trabajos"/"Ver Todos los Reportes" panels (still decorative mockups —
//               see that view's FILE SUMMARY), every list here is REAL and every action in
//               its 3 CRUD panels (Usuarios/Trabajos/Reportes) round-trips to the API:
//               Users (GET /api/users), Services (GET /api/services, the Admin-only
//               every-status listing — not GetOpenAsync/GetMineAsync), Reports
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
}
