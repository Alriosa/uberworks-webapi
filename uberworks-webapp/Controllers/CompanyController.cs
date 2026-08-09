// =====================================================================================
// FILE SUMMARY
// What it does: Company-only area of the WebApp. [Authorize(Roles = nameof(UserRole.Company))]
//               on the whole controller means anyone without the Company role gets
//               redirected/403'd before any action runs — same pattern as AdminController.cs.
//               CreateWorker/MyWorkers read the caller's own JWT from the "access_token"
//               claim and forward it as a Bearer token to the API's
//               POST /api/professionals/company-create and GET /api/professionals/my-workers,
//               which is what actually scopes everything to THIS Company's own account on
//               the API side. The Controller only handles HTTP/form concerns; all the
//               actual API communication goes through IProfessionalsApiClient, never
//               directly through HttpClient here.
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

[Authorize(Roles = nameof(UserRole.Company))]
public class CompanyController : Controller
{
    private readonly IProfessionalsApiClient _professionalsApiClient;

    public CompanyController(IProfessionalsApiClient professionalsApiClient)
    {
        _professionalsApiClient = professionalsApiClient;
    }

    [HttpGet]
    public IActionResult CreateWorker()
    {
        return View(new CreateWorkerViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateWorker(CreateWorkerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var accessToken = User.FindFirst("access_token")!.Value;

            await _professionalsApiClient.CreateWorkerAsync(accessToken, new CompanyCreateWorkerRequest
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Description = model.Description,
                Experience = model.Experience,
                Availability = model.Availability,
                Location = model.Location
            });

            TempData["SuccessMessage"] = $"Worker '{model.Username}' was created. We've emailed them a link to set their password.";
            return RedirectToAction(nameof(MyWorkers));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyWorkers()
    {
        var accessToken = User.FindFirst("access_token")!.Value;
        var workers = await _professionalsApiClient.GetMyWorkersAsync(accessToken);
        return View(workers);
    }
}
