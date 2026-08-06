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
// Entities connected: Professional.cs (indirectly, via IProfessionalService)
// Tables related: TBL_PROFESSIONALS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessionalsController : ControllerBase
{
    private readonly IProfessionalService _professionalService;
    private readonly ICurrentUserService _currentUserService;

    public ProfessionalsController(IProfessionalService professionalService, ICurrentUserService currentUserService)
    {
        _professionalService = professionalService;
        _currentUserService = currentUserService;
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProfessionalRequest request)
    {
        var result = await _professionalService.UpdateAsync(id, request);
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

    [HttpGet("my-workers")]
    [Authorize(Roles = nameof(UserRole.Company))]
    public async Task<IActionResult> MyWorkers()
    {
        var companyUserId = _currentUserService.UserId!.Value;
        var result = await _professionalService.GetByCompanyUserIdAsync(companyUserId);
        return Ok(result);
    }
}
