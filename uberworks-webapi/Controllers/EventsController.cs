// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the HTTP endpoints for the Company Event/invitation system. Create
// is Company-only ([Authorize(Roles = nameof(UserRole.Company))]) — a Manager dashboard
// deliberately has no "Crear Evento" button, per explicit request. GetMyEvents is usable by
// both Company and Manager (same company). GetMyInvitations/Respond are Professional-only —
// the receiving side of an invitation.
// Entities connected: Event.cs, EventInvitation.cs (indirectly, via IEventService)
// Tables related: TBL_EVENTS, TBL_EVENT_INVITATIONS (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ICurrentUserService _currentUserService;

    public EventsController(IEventService eventService, ICurrentUserService currentUserService)
    {
        _eventService = eventService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Company))]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        var companyUserId = _currentUserService.UserId!.Value;
        var companyUsername = _currentUserService.Username!;
        var result = await _eventService.CreateAsync(companyUserId, companyUsername, request);
        return CreatedAtAction(nameof(GetMyEvents), new { }, result);
    }

    /// <summary>Also usable by a Manager — always resolves to the SAME company they belong to.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = "Company,Manager")]
    public async Task<IActionResult> GetMyEvents()
    {
        var callerUserId = _currentUserService.UserId!.Value;
        var callerRole = _currentUserService.Role!.Value;
        var result = await _eventService.GetMyEventsAsync(callerUserId, callerRole);
        return Ok(result);
    }

    [HttpGet("invitations/mine")]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> GetMyInvitations()
    {
        var professionalUserId = _currentUserService.UserId!.Value;
        var result = await _eventService.GetMyInvitationsAsync(professionalUserId);
        return Ok(result);
    }

    [HttpPost("invitations/{id:int}/respond")]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> RespondToInvitation(int id, [FromBody] RespondToInvitationRequest request)
    {
        var professionalUserId = _currentUserService.UserId!.Value;
        var result = await _eventService.RespondToInvitationAsync(professionalUserId, id, request.Accept);
        return Ok(result);
    }
}
