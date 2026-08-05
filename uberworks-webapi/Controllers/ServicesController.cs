// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for the "Work Post" (create, list open ones,
//               list mine, view detail). GetById is the only one that can be called without
//               being logged in (that's why it has no [Authorize]) — the exact address's
//               privacy is decided internally by IServiceService based on
//               _currentUserService.UserId (which can be null here if no one is logged in).
// Entities connected: Service.cs (indirectly, via IServiceService)
// Tables related: TBL_SERVICES (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly ICurrentUserService _currentUserService;

    public ServicesController(IServiceService serviceService, ICurrentUserService currentUserService)
    {
        _serviceService = serviceService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Client))]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
    {
        var clientId = _currentUserService.UserId!.Value;
        var result = await _serviceService.CreateAsync(clientId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Public listing so professionals can browse open requests (no exact address).</summary>
    [HttpGet("open")]
    public async Task<IActionResult> GetOpen()
    {
        var result = await _serviceService.GetOpenAsync();
        return Ok(result);
    }

    [HttpGet("mine")]
    [Authorize(Roles = nameof(UserRole.Client))]
    public async Task<IActionResult> GetMine()
    {
        var clientId = _currentUserService.UserId!.Value;
        var result = await _serviceService.GetMyServicesAsync(clientId);
        return Ok(result);
    }

    /// <summary>
    /// Open to anyone (even anonymous) so the public detail of a post can be viewed.
    /// The exact address is only included if the caller is the owner or the accepted professional.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _serviceService.GetByIdAsync(id, _currentUserService.UserId);
        return Ok(result);
    }
}
