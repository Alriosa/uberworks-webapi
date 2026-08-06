// =====================================================================================
// FILE SUMMARY
// What it does: Shows the logged-in user's own profile. [Authorize] (no specific role) —
//               anyone logged in can see their own profile, just a DIFFERENT view per role
//               (Views/Profile/Client.cshtml, Professional.cshtml, Company.cshtml,
//               Manager.cshtml, Admin.cshtml, MasterAdmin.cshtml — one file per role,
//               deliberately, instead of one shared view full of @if (User.IsInRole(...))
//               conditionals that would be easy to get wrong). Index() reads the caller's
//               own id/role from the "access_token" JWT claim, calls GET /api/users/{id} for
//               the base data every role shares (ProfileViewModelBase.cs), then — only for
//               Professional/Company — makes ONE extra call for that role's specific data
//               (the worker profile, or the worker count) before picking which View() to
//               render.
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
public class ProfileController : Controller
{
    private readonly IUsersApiClient _usersApiClient;
    private readonly IProfessionalsApiClient _professionalsApiClient;

    public ProfileController(IUsersApiClient usersApiClient, IProfessionalsApiClient professionalsApiClient)
    {
        _usersApiClient = usersApiClient;
        _professionalsApiClient = professionalsApiClient;
    }

    public async Task<IActionResult> Index()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        var user = await _usersApiClient.GetByIdAsync(accessToken, userId);

        switch (role)
        {
            case UserRole.Professional:
                var professional = await _professionalsApiClient.GetByUserIdAsync(userId);
                return View("Professional", MapTo<ProfessionalProfileViewModel>(user, vm =>
                {
                    vm.Description = professional.Description;
                    vm.Experience = professional.Experience;
                    vm.Availability = professional.Availability;
                    vm.Location = professional.Location;
                    vm.AverageRating = professional.AverageRating;
                    vm.CompanyUserId = professional.CompanyUserId;
                }));

            case UserRole.Company:
                var workers = await _professionalsApiClient.GetMyWorkersAsync(accessToken);
                return View("Company", MapTo<CompanyProfileViewModel>(user, vm => vm.WorkerCount = workers.Count));

            case UserRole.Manager:
                return View("Manager", MapTo<ManagerProfileViewModel>(user));

            case UserRole.Admin:
                return View("Admin", MapTo<AdminProfileViewModel>(user));

            case UserRole.MasterAdmin:
                return View("MasterAdmin", MapTo<MasterAdminProfileViewModel>(user));

            default:
                return View("Client", MapTo<ClientProfileViewModel>(user));
        }
    }

    private static TViewModel MapTo<TViewModel>(UserResponse user, Action<TViewModel>? configure = null)
        where TViewModel : ProfileViewModelBase, new()
    {
        var viewModel = new TViewModel
        {
            Id = user.Id,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            RegistrationDate = user.RegistrationDate
        };

        configure?.Invoke(viewModel);
        return viewModel;
    }
}
