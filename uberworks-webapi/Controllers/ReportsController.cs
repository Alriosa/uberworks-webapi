// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for the dispute/report system. Most actions are
// restricted to [Authorize(Roles = "MasterAdmin,Admin,Support")] — internal tooling (Admin
// dashboard's report CRUD panel, Support dashboard). ContactSupport is the one exception —
// [Authorize(Roles = "Client")] only — it backs the Client dashboard's real "Contactar con
// Soporte" self-service view, per explicit request. There used to be a single class-level
// [Authorize] covering every action; it was replaced with one per action so ContactSupport
// could have its own, narrower rule (ASP.NET Core combines a class-level and method-level
// [Authorize] with AND, not OR, so a shared class-level attribute would have made it
// impossible to let Client in on just one action).
// Create/ContactSupport are the only places this Controller touches infrastructure directly
// (same reasoning as ProfessionalsController.UploadPhoto): SaveImagesAsync() saves each
// uploaded image to LOCAL disk under wwwroot/uploads/report-images (served back via
// app.UseStaticFiles(), same plan to move to external storage later) and only hands
// IReportService the resulting relative URLs.
// Entities connected: Report.cs (indirectly, via IReportService)
// Tables related: TBL_REPORTS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    // Same limits as ProfessionalsController's photo upload.
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public ReportsController(IReportService reportService, ICurrentUserService currentUserService, IWebHostEnvironment webHostEnvironment)
    {
        _reportService = reportService;
        _currentUserService = currentUserService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> Create([FromForm] CreateReportRequest request)
    {
        var imageUrls = await SaveImagesAsync(request.Images);
        var createdByUserId = _currentUserService.UserId!.Value;
        var result = await _reportService.CreateAsync(createdByUserId, request, imageUrls);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// The Client dashboard's real "Contactar con Soporte" view — title, body text, optional
    /// image upload, optional associated Service/case id, per explicit request ("todos los
    /// trabajos deben tener un ID... uno agarra el ID de ese trabajo y se lo presenta a
    /// soporte"). ClientUserId is always the caller (see ReportService.CreateFromClientAsync)
    /// — never something the client can set themselves.
    /// </summary>
    [HttpPost("contact-support")]
    [Authorize(Roles = nameof(UserRole.Client))]
    public async Task<IActionResult> ContactSupport([FromForm] ClientCreateReportRequest request)
    {
        var imageUrls = await SaveImagesAsync(request.Images);
        var clientUserId = _currentUserService.UserId!.Value;
        var result = await _reportService.CreateFromClientAsync(clientUserId, request, imageUrls);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reportService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _reportService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReportRequest request)
    {
        var result = await _reportService.UpdateAsync(id, request);
        return Ok(result);
    }

    /// <summary>"Resolver" button on the Support dashboard.</summary>
    [HttpPost("{id:int}/resolve")]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> Resolve(int id, [FromBody] ResolveReportRequest request)
    {
        var resolvedByUserId = _currentUserService.UserId!.Value;
        var result = await _reportService.ResolveAsync(id, resolvedByUserId, request);
        return Ok(result);
    }

    /// <summary>"Fallo a favor de nadie" button on the Support dashboard.</summary>
    [HttpPost("{id:int}/no-fault")]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> NoFault(int id)
    {
        var resolvedByUserId = _currentUserService.UserId!.Value;
        var result = await _reportService.NoFaultAsync(id, resolvedByUserId);
        return Ok(result);
    }

    /// <summary>
    /// "Cancelar reporte" on the Support dashboard AND "Borrar" on the Admin dashboard's
    /// report CRUD panel — the same operation either way (see IReportService.CancelAsync).
    /// </summary>
    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "MasterAdmin,Admin,Support")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelReportRequest request)
    {
        var resolvedByUserId = _currentUserService.UserId!.Value;
        var result = await _reportService.CancelAsync(id, resolvedByUserId, request);
        return Ok(result);
    }

    private async Task<List<string>> SaveImagesAsync(List<IFormFile>? images)
    {
        var imageUrls = new List<string>();

        if (images is null)
        {
            return imageUrls;
        }

        foreach (var image in images)
        {
            if (image.Length == 0)
            {
                continue;
            }

            if (image.Length > MaxImageSizeBytes)
            {
                throw new ArgumentException("Each image must be 5 MB or smaller.");
            }

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Only .jpg, .jpeg, .png, or .webp images are allowed.");
            }

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "report-images");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            imageUrls.Add($"/uploads/report-images/{fileName}");
        }

        return imageUrls;
    }
}
