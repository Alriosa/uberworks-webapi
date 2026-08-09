// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for the Penalty ("Advertencias"/sanctions)
//               system. GetMine is open to any authenticated user — it backs a user
//               checking their own record (Professional dashboard's "Advertencias" modal
//               today, per explicit request). GetForUser/GetAll/Create are
//               Admin/MasterAdmin-only — applying a sanction and browsing everyone's.
// Entities connected: Penalty.cs (indirectly, via IPenaltyService)
// Tables related: TBL_PENALTIES (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/penalties")]
public class PenaltiesController : ControllerBase
{
    private readonly IPenaltyService _penaltyService;
    private readonly ICurrentUserService _currentUserService;

    public PenaltiesController(IPenaltyService penaltyService, ICurrentUserService currentUserService)
    {
        _penaltyService = penaltyService;
        _currentUserService = currentUserService;
    }

    /// <summary>Any authenticated user checking their own sanctions/warnings.</summary>
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine()
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _penaltyService.GetForUserAsync(userId);
        return Ok(result);
    }

    [HttpGet("user/{userId:int}")]
    [Authorize(Roles = "MasterAdmin,Admin")]
    public async Task<IActionResult> GetForUser(int userId)
    {
        var result = await _penaltyService.GetForUserAsync(userId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "MasterAdmin,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _penaltyService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "MasterAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePenaltyRequest request)
    {
        var result = await _penaltyService.CreateAsync(request);
        return CreatedAtAction(nameof(GetForUser), new { userId = result.UserId }, result);
    }
}
