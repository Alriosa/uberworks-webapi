// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/MasterAdmin.cshtml. Users is the REAL, full user
//               directory (GET /api/users via IUsersApiClient.GetAllUsersAsync) shown in the
//               "Ver Todos los Usuarios" panel — every attribute per user, since the user
//               asked to see everything first and decide later what's worth keeping visible.
//               "Trabajos" and "Reportes" on that same dashboard are still decorative mockups
//               (no Job/Report entity exists yet) and "Tarifas & Configuración" does nothing
//               yet — those three are intentionally NOT backed by anything in this view model.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class MasterAdminDashboardViewModel : DashboardViewModelBase
{
    public List<AdminUserListItemResponse> Users { get; set; } = new();
}
