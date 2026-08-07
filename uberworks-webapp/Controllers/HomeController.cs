// =====================================================================================
// FILE SUMMARY
// What it does: LandingPage is the public marketing homepage (see its own FILE SUMMARY for
//               why it's the one Spanish/light-theme/Tailwind page in the app). AllServices
//               is the "browse everything" page the "Y más servicios" card links to — same
//               _LandingLayout.cshtml and _LandingHeader.cshtml partial, just a plain grid of
//               every service category instead of the curated 6 with photos. AllServices also
//               renders the "suggest a service" modal, whose submit posts to SuggestService,
//               which forwards the form (including the optional file attachment) to the API
//               via IContactApiClient — this is a REAL email send, not decorative, so failures
//               from the API (e.g. attachment too large) are surfaced back as a TempData error
//               instead of silently succeeding. Privacy/Error are the untouched framework
//               template pages.
// Entities connected: None
// Tables related: None
// =====================================================================================
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Models;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

public class HomeController : Controller
{
    private readonly IContactApiClient _contactApiClient;

    public HomeController(IContactApiClient contactApiClient)
    {
        _contactApiClient = contactApiClient;
    }

    public IActionResult LandingPage()
    {
        return View();
    }

    public IActionResult AllServices()
    {
        return View(new SuggestServiceViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuggestService(SuggestServiceViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["SuggestServiceError"] = "Please fill in your name, a valid email, and a message.";
            return RedirectToAction(nameof(AllServices));
        }

        try
        {
            await _contactApiClient.SuggestServiceAsync(
                model.Name,
                model.IsFromCompany,
                model.CompanyName,
                model.Email,
                model.Message,
                model.Attachment);

            TempData["SuggestServiceSuccess"] = "¡Gracias! Recibimos tu sugerencia y la vamos a revisar.";
        }
        catch (ApiException ex)
        {
            TempData["SuggestServiceError"] = ex.Message;
        }

        return RedirectToAction(nameof(AllServices));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
