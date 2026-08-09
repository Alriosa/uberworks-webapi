// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Client.cshtml. Both MyServices (from
//               GET /api/services/mine) and CurrentUser (from GET /api/users/{id}, self) are
//               REAL data — MyServices backs the "Histórico de trabajos" modal, CurrentUser
//               pre-fills the "Editar mi perfil" modal (which posts to
//               DashboardController.UpdateProfile, then PUT /api/users/{id}).
//               "Ocupo un trabajo" links to Home/AllServices (the existing service-browsing
//               page) rather than a real service-request form — POST /api/services exists on
//               the API but there's no WebApp form wired to it yet. "Contactar con soporte"
//               and "Recargar saldo" are still decorative (no support-ticket or wallet/balance
//               system exists in the app).
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_SERVICES/TBL_USERS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ClientDashboardViewModel : DashboardViewModelBase
{
    public List<ServiceResponse> MyServices { get; set; } = new();
    public UserResponse CurrentUser { get; set; } = new();
    public EditProfileViewModel EditProfile { get; set; } = new();
}
