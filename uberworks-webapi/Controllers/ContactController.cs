// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the "suggest a service" contact form endpoint. No [Authorize] on
//               purpose — anyone visiting the public site (logged in or not) can submit
//               this. Binds via [FromForm] instead of the usual [FromBody] because the
//               request includes a file (ServiceSuggestionRequest.Attachment), which needs
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
}
