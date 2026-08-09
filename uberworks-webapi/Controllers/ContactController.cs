// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the "suggest a service" contact form endpoint AND the general
//               "Contáctanos" page's message endpoint (linked from the site footer). Neither
//               has [Authorize] — anyone visiting the public site (logged in or not) can
//               submit either. Both bind via [FromForm] instead of the usual [FromBody]
//               because each request includes an optional file, which needs
//               multipart/form-data, not JSON.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost("suggest-service")]
    public async Task<IActionResult> SuggestService([FromForm] ServiceSuggestionRequest request)
    {
        await _contactService.SuggestServiceAsync(request);
        return Ok(new { message = "Thanks! We've received your suggestion." });
    }

    /// <summary>The general "Contáctanos" page (footer link) — no [Authorize], same as SuggestService above.</summary>
    [HttpPost("message")]
    public async Task<IActionResult> SendMessage([FromForm] ContactUsRequest request)
    {
        await _contactService.SendContactMessageAsync(request);
        return Ok(new { message = "Thanks! We've received your message." });
    }
}
