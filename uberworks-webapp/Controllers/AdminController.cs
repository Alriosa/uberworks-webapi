// =====================================================================================
// FILE SUMMARY
// What it does: Admin-only area of the WebApp. [Authorize(Roles = "MasterAdmin,Admin,Manager")]
//               on the whole controller means a Client/Professional/Company (or an
//               anonymous visitor) gets redirected to /Account/Login (or a 403, if already
//               logged in with the wrong role) before any action runs — MVC's cookie auth
//               checks the "Role" claim set at login time (see AccountController.SignInAsync).
//               CreateUser reads the caller's own JWT from the "access_token" claim on that
//               same cookie and forwards it as a Bearer token to POST /api/users/admin-create,
//               which is what actually authorizes the write on the API side. The Role
//               dropdown only shows options RoleHierarchy.cs says this caller's role can
//               create (UI convenience only — the API enforces the real rule regardless).
//               There is no Password field on this form on purpose: the API creates the
//               account with IsPasswordSet=false and emails the new user a real
//               "set your password" link (reusing the forgot-password token flow) — nobody
//               but that person should ever know their own password.
//               The Controller only handles HTTP/form concerns; all the actual API
//               communication goes through IUsersApiClient, never directly through
//               HttpClient here.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS only indirectly, through the API
// =====================================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using uberworks_webapp.Common;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

[Authorize(Roles = "MasterAdmin,Admin,Manager")]
public class AdminController : Controller
{
    private readonly IUsersApiClient _usersApiClient;

    public AdminController(IUsersApiClient usersApiClient)
    {
        _usersApiClient = usersApiClient;
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        ViewData["RoleOptions"] = BuildRoleOptions();
        return View(new AdminCreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(AdminCreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["RoleOptions"] = BuildRoleOptions();
            return View(model);
        }

        try
        {
            var accessToken = User.FindFirst("access_token")!.Value;

            await _usersApiClient.AdminCreateUserAsync(accessToken, new AdminCreateUserRequest
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Role = model.Role
            });

            TempData["SuccessMessage"] = $"User '{model.Username}' was created. We've emailed {model.Email} a link to set their password.";
            return RedirectToAction(nameof(CreateUser));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewData["RoleOptions"] = BuildRoleOptions();
            return View(model);
        }
    }

    private List<SelectListItem> BuildRoleOptions()
    {
        var actorRole = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);

        return RoleHierarchy.GetCreatableRoles(actorRole)
            .Select(role => new SelectListItem(RoleHierarchy.GetDisplayLabel(role), ((int)role).ToString()))
            .ToList();
    }
}
