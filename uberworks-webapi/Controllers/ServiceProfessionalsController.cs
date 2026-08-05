// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Expone todo el ciclo de vida de la negociación de un Service: proponer, ver
//           propuestas, aceptar, confirmar llegada, subir foto de evidencia, y confirmar
//           cierre mutuo. Todas las rutas cuelgan de /api/services/{serviceId}/... y cada
//           una tiene el [Authorize(Roles=...)] correspondiente según quién debe poder
//           llamarla (Client vs Professional vs cualquiera de los dos ya logueado).
// Entidades relacionadas: ServiceProfessional.cs, Service.cs (indirectamente, vía
//                          IServiceProfessionalService)
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS, TBL_SERVICES (indirectamente)
// =====================================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

/// <summary>
/// Ciclo de vida de la negociación de un Service: propuestas, aceptación,
/// check-in de llegada, evidencia y confirmación mutua de cierre.
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

    /// <summary>Solo el cliente dueño del servicio puede ver todas las propuestas recibidas.</summary>
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

    /// <summary>Botón "Estoy en el sitio". El timestamp lo pone el servidor.</summary>
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
    /// Pantalla "¿Trabajo terminado?" — la llama tanto el cliente como el profesional
    /// (cada quien desde su propia sesión). Cuando ambos confirmaron, el Service se cierra.
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
