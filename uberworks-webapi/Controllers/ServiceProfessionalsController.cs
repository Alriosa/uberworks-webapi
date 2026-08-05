// =====================================================================================
// FILE SUMMARY
// What it does: Exposes a Service's full negotiation lifecycle: propose, view proposals,
//               accept, confirm arrival, upload completion evidence, and confirm mutual
//               closing. All routes hang off /api/services/{serviceId}/... and each one has
//               the matching [Authorize(Roles=...)] depending on who should be able to call
//               it (Client vs Professional vs either one already logged in).
// Entities connected: ServiceProfessional.cs, Service.cs (indirectly, via
//                      IServiceProfessionalService)
// Tables related: TBL_SERVICE_PROFESSIONALS, TBL_SERVICES (indirectly)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

/// <summary>
/// A Service's negotiation lifecycle: proposals, acceptance,
/// arrival check-in, evidence, and mutual closing confirmation.
/// </summary>
[ApiController]
[Route("api/services/{serviceId:int}")]
public class ServiceProfessionalsController : ControllerBase
{
    private readonly IServiceProfessionalService _serviceProfessionalService;
    private readonly ICurrentUserService _currentUserService;

    public ServiceProfessionalsController(
        IServiceProfessionalService serviceProfessionalService,
        ICurrentUserService currentUserService)
    {
        _serviceProfessionalService = serviceProfessionalService;
        _currentUserService = currentUserService;
    }

    [HttpPost("proposals")]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> CreateProposal(int serviceId, [FromBody] CreateServiceProfessionalRequest request)
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _serviceProfessionalService.CreateProposalAsync(userId, serviceId, request);
        return CreatedAtAction(nameof(GetProposals), new { serviceId }, result);
    }

    /// <summary>Only the client who owns the service can see all received proposals.</summary>
    [HttpGet("proposals")]
    [Authorize(Roles = nameof(UserRole.Client))]
    public async Task<IActionResult> GetProposals(int serviceId)
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _serviceProfessionalService.GetProposalsAsync(userId, serviceId);
        return Ok(result);
    }

    [HttpPost("proposals/{proposalId:int}/accept")]
    [Authorize(Roles = nameof(UserRole.Client))]
    public async Task<IActionResult> Accept(int serviceId, int proposalId)
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _serviceProfessionalService.AcceptProposalAsync(userId, serviceId, proposalId);
        return Ok(result);
    }

    /// <summary>"I'm on site" button. The timestamp is set by the server.</summary>
    [HttpPost("arrival")]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> ConfirmArrival(int serviceId)
    {
        var userId = _currentUserService.UserId!.Value;
        await _serviceProfessionalService.ConfirmArrivalAsync(userId, serviceId);
        return NoContent();
    }

    [HttpPost("completion-photo")]
    [Authorize(Roles = nameof(UserRole.Professional))]
    public async Task<IActionResult> UploadCompletionPhoto(int serviceId, [FromBody] UploadCompletionPhotoRequest request)
    {
        var userId = _currentUserService.UserId!.Value;
        await _serviceProfessionalService.UploadCompletionPhotoAsync(userId, serviceId, request.PhotoUrl);
        return NoContent();
    }

    /// <summary>
    /// The "Is the job done?" screen — called by both the client and the professional
    /// (each from their own session). Once both confirm, the Service closes.
    /// </summary>
    [HttpPost("confirm-completion")]
    [Authorize]
    public async Task<IActionResult> ConfirmCompletion(int serviceId)
    {
        var userId = _currentUserService.UserId!.Value;
        var result = await _serviceProfessionalService.ConfirmCompletionAsync(userId, serviceId);
        return Ok(result);
    }
}
