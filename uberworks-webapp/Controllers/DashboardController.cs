// =====================================================================================
// FILE SUMMARY
// What it does: The landing page after a successful login (see AccountController.Login/
//               GoogleCallback, which redirect here instead of Home/Index). [Authorize]
//               (no specific role) — LandingPage() reads the caller's own name/id/role from the
//               "access_token" JWT claim, and picks a COMPLETELY DIFFERENT view per role
//               (Views/Dashboard/Client.cshtml, Professional.cshtml, Company.cshtml,
//               Manager.cshtml, Admin.cshtml, MasterAdmin.cshtml — one file per role, same
//               reasoning as ProfileController.cs). Each dashboard only shows summary data
//               that already exists today (no invented Services/Payments numbers) — a
//               Company sees its real worker count, a Professional sees its real average
//               rating, everyone else gets a welcome + quick links to what they can
//               actually do.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS/TBL_PROFESSIONALS only indirectly, through the API
// =====================================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IProfessionalsApiClient _professionalsApiClient;

    public DashboardController(IProfessionalsApiClient professionalsApiClient)
    {
        _professionalsApiClient = professionalsApiClient;
    }

    public async Task<IActionResult> LandingPage()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);
        // AppClaimsFactory.cs sets ClaimTypes.Name to the Username (not FirstName) — that's
        // what's actually available on the claims principal without an extra API call.
        var displayName = User.Identity?.Name ?? "there";

        switch (role)
        {
            case UserRole.Professional:
                var professional = await _professionalsApiClient.GetByUserIdAsync(userId);
                return View("Professional", new ProfessionalDashboardViewModel
                {
                    DisplayName = displayName,
                    AverageRating = professional.AverageRating
                });

            case UserRole.Company:
                var workers = await _professionalsApiClient.GetMyWorkersAsync(accessToken);
                return View("Company", new CompanyDashboardViewModel
                {
                    DisplayName = displayName,
                    WorkerCount = workers.Count
                });

            case UserRole.Manager:
                return View("Manager", new ManagerDashboardViewModel { DisplayName = displayName });

            case UserRole.Admin:
                return View("Admin", new AdminDashboardViewModel { DisplayName = displayName });

            case UserRole.MasterAdmin:
                return View("MasterAdmin", new MasterAdminDashboardViewModel { DisplayName = displayName });

            default:
                return View("Client", new ClientDashboardViewModel { DisplayName = displayName });
        }
    }
}
