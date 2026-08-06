// =====================================================================================
// FILE SUMMARY
// What it does: Admin-only area of the WebApp. [Authorize(Roles = "MasterAdmin,Admin")] on
//               the whole controller means a Client/Professional (or an anonymous visitor)
//               gets redirected to /Account/Login (or a 403, if already logged in with the
//               wrong role) before any action runs — MVC's cookie auth checks the "Role"
//               claim set at login time (see AccountController.SignInAsync). CreateUser
//               reads the caller's own JWT from the "access_token" claim on that same cookie
//               and forwards it as a Bearer token to POST /api/users/admin-create, which is
//               what actually authorizes the write on the API side. The Controller only
//               handles HTTP/form concerns; all the actual API communication goes through
//               IUsersApiClient, never directly through HttpClient here.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS only indirectly, through the API
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

[Authorize(Roles = "MasterAdmin,Admin")]
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
        return View(new AdminCreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(AdminCreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
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
                Password = model.Password,
                Role = model.Role
            });

            TempData["SuccessMessage"] = $"User '{model.Username}' was created successfully.";
            return RedirectToAction(nameof(CreateUser));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}
