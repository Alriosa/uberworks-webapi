// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for Professional. It receives the request,
//               validates it at a shape level ([Authorize] checks the role before the code
//               enters the method), and delegates ALL the real logic to IProfessionalService
//               — the Controller never decides business rules, it just translates
//               HTTP <-> calls to Services. CompanyCreateWorker/MyWorkers are restricted to
//               [Authorize(Roles = nameof(UserRole.Company))] and always act on the CALLER's
//               own CompanyUserId (from the JWT, via ICurrentUserService) — a Company can
//               never create or list workers under a different Company's account.
//               UploadPhoto is the one place this Controller DOES touch infrastructure
//               directly (saving the uploaded file to disk) instead of delegating everything
//               to the Service — deliberately, since "where a file physically lives" is an
//               HTTP/hosting concern, not a business rule. It saves under
//               wwwroot/uploads/professional-photos on LOCAL disk for now (served back via
//               app.UseStaticFiles() in Program.cs); the plan is to swap this one method for
//               a call to external storage (S3/Azure Blob/etc.) later without touching
//               IProfessionalService at all — PhotoUrl is already just "whatever URL the
//               photo lives at", so the rest of the app doesn't care where that ends up.
// Entities connected: Professional.cs (indirectly, via IProfessionalService)
// Tables related: TBL_PROFESSIONALS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessionalsController : ControllerBase
{
    private readonly IProfessionalService _professionalService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    // Deliberately small — a profile photo has no business being 10+ MB. Matches the same
    // idea as ContactService.MaxAttachmentSizeBytes.
    private const long MaxPhotoSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public ProfessionalsController(
        IProfessionalService professionalService,
        ICurrentUserService currentUserService,
        IWebHostEnvironment webHostEnvironment)
    {
        _professionalService = professionalService;
        _currentUserService = currentUserService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> Create([FromBody] CreateProfessionalRequest request)
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _professionalService.CreateAsync(userId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _professionalService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("by-user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var result = await _professionalService.GetByUserIdAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// The real, distinct list (up to 3) of WorkType categories the caller has actually had a
    /// proposal accepted/completed on — backs the "trabajos que puede realizar" section on
    /// the Professional profile page. Always acts on the CALLER's own professional profile.
    /// </summary>
    [HttpGet("my-accepted-worktypes")]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> GetMyAcceptedWorkTypes()
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _professionalService.GetAcceptedWorkTypesAsync(userId);
        return Ok(result);
    }

    /// <summary>Only the profile owner or an Admin/MasterAdmin can edit it.</summary>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProfessionalRequest request)
    {
        var callerUserId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;
        var result = await _professionalService.UpdateAsync(id, callerUserId, callerRole, request);
        return Ok(result);
    }

    /// <summary>
    /// Uploads/replaces the caller's own profile photo. Saved to LOCAL disk under
    /// wwwroot/uploads/professional-photos for now (see the Controller's FILE SUMMARY for the
    /// plan to move this to external storage later) — served back via app.UseStaticFiles().
    /// Only the profile owner or an Admin/MasterAdmin can change it (same rule as Update).
    /// </summary>
    [HttpPost("{id:int}/photo")]
    [Authorize]
    public async Task<IActionResult> UploadPhoto(int id, [FromForm] IFormFile photo)
    {
        if (photo.Length == 0)
        {
            throw new ArgumentException("No photo was uploaded.");
        }

        if (photo.Length > MaxPhotoSizeBytes)
        {
            throw new ArgumentException("The photo is too large (max 5 MB).");
        }

        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Only .jpg, .jpeg, .png, or .webp photos are allowed.");
        }

        var callerUserId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;

        // A random file name (not the professional's id alone) avoids overwriting collisions
        // and avoids leaking any information through the URL itself.
        var fileName = $"{id}-{Guid.NewGuid():N}{extension}";
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "professional-photos");
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        var photoUrl = $"/uploads/professional-photos/{fileName}";
        var result = await _professionalService.UpdatePhotoAsync(id, callerUserId, callerRole, photoUrl);
        return Ok(result);
    }

    [HttpPost("company-create")]
    [Authorize(Roles = nameof(UserRole.Company))]
    public async Task<IActionResult> CompanyCreateWorker([FromBody] CompanyCreateWorkerRequest request)
    {
        var companyUserId = _currentUserService.UserId!.Value;
        var companyUsername = _currentUserService.Username!;
        var result = await _professionalService.CreateByCompanyAsync(companyUserId, companyUsername, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Also usable by a Manager — always resolves to the SAME company they belong to.</summary>
    [HttpGet("my-workers")]
    [Authorize(Roles = "Company,Manager")]
    public async Task<IActionResult> MyWorkers()
    {
        var callerUserId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;
        var result = await _professionalService.GetMyCompanyWorkersAsync(callerUserId, callerRole);
        return Ok(result);
    }

    /// <summary>
    /// Links an EXISTING Professional account to the caller's company, searched by
    /// email/username/phone. Also usable by a Manager (same company-resolution rule as
    /// MyWorkers above).
    /// </summary>
    [HttpPost("link-existing")]
    [Authorize(Roles = "Company,Manager")]
    public async Task<IActionResult> LinkExisting([FromBody] LinkWorkerRequest request)
    {
        var callerUserId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;
        var result = await _professionalService.LinkExistingWorkerAsync(callerUserId, callerRole, request.Contact);
        return Ok(result);
    }

    /// <summary>Removes a worker from the caller's company (CompanyUserId set back to null).</summary>
    [HttpPost("{id:int}/unlink")]
    [Authorize(Roles = "Company,Manager")]
    public async Task<IActionResult> Unlink(int id)
    {
        var callerUserId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;
        await _professionalService.UnlinkWorkerAsync(callerUserId, callerRole, id);
        return NoContent();
    }
}
