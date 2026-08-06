// =====================================================================================
// FILE SUMMARY
// What it does: Default MVC scaffold controller (Home/Privacy/Error pages). Not tied to
//               the API in any way — just the framework template pages. Kept as-is; the
//               real app pages live under Controllers/AccountController.cs and whatever
//               gets built next.
// Entities connected: None
// Tables related: None
// =====================================================================================
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Models;

namespace uberworks_webapp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
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
