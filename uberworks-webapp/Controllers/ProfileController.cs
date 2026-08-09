// =====================================================================================
// FILE SUMMARY
// What it does: Shows the logged-in user's own profile. [Authorize] (no specific role) —
//               anyone logged in can see their own profile, just a DIFFERENT view per role
//               (Views/Profile/Client.cshtml, ProfileProfessional.cshtml, Company.cshtml,
//               Manager.cshtml, Admin.cshtml, MasterAdmin.cshtml — one file per role,
//               deliberately, instead of one shared view full of @if (User.IsInRole(...))
//               conditionals that would be easy to get wrong). LandingPage() reads the
//               caller's own id/role from the "access_token" JWT claim, calls
//               GET /api/users/{id} for the base data every role shares
//               (ProfileViewModelBase.cs), then — only for Professional/Company — makes
//               extra calls for that role's specific data before picking which View() to
//               render. For Professional, that's the worker profile (GET /api/professionals/
//               by-user/{userId}) AND the real "trabajos que puede realizar" list
//               (GET /api/professionals/my-accepted-worktypes).
//               UpdateProfessionalProfile backs ProfileProfessional.cshtml's "Editar"/
//               "Guardar" modal — it updates BOTH the User (FirstName/LastName/Phone, via
//               IUsersApiClient) and the Professional (Description/Experience/Availability/
//               Location, via IProfessionalsApiClient) in one POST, since the form shows
//               fields from both resources but only has one save button.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS/TBL_PROFESSIONALS only indirectly, through the API
// =====================================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUsersApiClient _usersApiClient;
    private readonly IProfessionalsApiClient _professionalsApiClient;
    private readonly IConfiguration _configuration;

    public ProfileController(IUsersApiClient usersApiClient, IProfessionalsApiClient professionalsApiClient, IConfiguration configuration)
    {
        _usersApiClient = usersApiClient;
        _professionalsApiClient = professionalsApiClient;
        _configuration = configuration;
    }

    public async Task<IActionResult> LandingPage()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        var user = await _usersApiClient.GetByIdAsync(accessToken, userId);

        switch (role)
        {
            case UserRole.Professional:
                var professional = await _professionalsApiClient.GetByUserIdAsync(userId);
                var acceptedWorkTypes = await _professionalsApiClient.GetMyAcceptedWorkTypesAsync(accessToken);
                return View("ProfileProfessional", MapTo<ProfessionalProfileViewModel>(user, vm =>
                {
                    vm.ProfessionalId = professional.Id;
                    vm.Description = professional.Description;
                    vm.Experience = professional.Experience;
                    vm.Availability = professional.Availability;
                    vm.Location = professional.Location;
                    vm.AverageRating = professional.AverageRating;
                    vm.CompanyUserId = professional.CompanyUserId;
                    vm.AcceptedWorkTypes = acceptedWorkTypes;
                    vm.PhotoUrl = professional.PhotoUrl;
                    vm.ApiBaseUrl = _configuration["Api:BaseUrl"] ?? string.Empty;
                    vm.EditProfile = new EditProfessionalProfileViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Phone = user.Phone,
                        Description = professional.Description,
                        Experience = professional.Experience,
                        Availability = professional.Availability,
                        Location = professional.Location
                    };
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

    /// <summary>
    /// Backs ProfileProfessional.cshtml's "Editar" modal. Updates the User's
    /// FirstName/LastName/Phone (PUT /api/users/{id}) and the Professional's
    /// Description/Experience/Availability/Location (PUT /api/professionals/{id}) — two API
    /// calls, one form.
    /// The parameter MUST be named "editProfile" (case-insensitive) — see the identical note
    /// on DashboardController.UpdateProfile for why a mismatched name silently leaves every
    /// field empty instead of throwing.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Professional")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfessionalProfile(EditProfessionalProfileViewModel editProfile, int professionalId)
    {
        if (!ModelState.IsValid)
        {
            TempData["EditProfileError"] = "Please fill in your first and last name.";
            return RedirectToAction(nameof(LandingPage));
        }

        var accessToken = User.FindFirst("access_token")!.Value;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            await _usersApiClient.UpdateAsync(accessToken, userId, new UpdateUserRequest
            {
                FirstName = editProfile.FirstName,
                LastName = editProfile.LastName,
                Phone = editProfile.Phone
            });

            await _professionalsApiClient.UpdateAsync(accessToken, professionalId, new UpdateProfessionalRequest
            {
                Description = editProfile.Description,
                Experience = editProfile.Experience,
                Availability = editProfile.Availability,
                Location = editProfile.Location
            });

            TempData["EditProfileSuccess"] = "Your profile was updated.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(LandingPage));
    }

    /// <summary>
    /// Backs ProfileProfessional.cshtml's "Cambiar Foto" form — a separate multipart form
    /// from the main "Editar" modal, since a file upload needs enctype="multipart/form-data"
    /// while the rest of the profile edits post plain form fields.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Professional")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfessionalPhoto(IFormFile photo, int professionalId)
    {
        if (photo is null || photo.Length == 0)
        {
            TempData["EditProfileError"] = "Please choose a photo to upload.";
            return RedirectToAction(nameof(LandingPage));
        }

        var accessToken = User.FindFirst("access_token")!.Value;

        try
        {
            await _professionalsApiClient.UploadPhotoAsync(accessToken, professionalId, photo);
            TempData["EditProfileSuccess"] = "Your profile photo was updated.";
        }
        catch (ApiException ex)
        {
            TempData["EditProfileError"] = ex.Message;
        }

        return RedirectToAction(nameof(LandingPage));
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
