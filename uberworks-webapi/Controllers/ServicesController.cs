// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Expone los endpoints HTTP del "Work Post" (crear, listar abiertos, listar los
//           míos, ver detalle). GetById es el único que se puede llamar sin estar logueado
//           (por eso no tiene [Authorize]) — la privacidad de la dirección exacta la decide
//           IServiceService internamente según _currentUserService.UserId (que aquí puede
//           ser null si nadie inició sesión).
// Entidades relacionadas: Service.cs (indirectamente, vía IServiceService)
// Tablas relacionadas: TBL_SERVICES (indirectamente, a través de todas las capas)
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

    /// <summary>Listado público para que los profesionales exploren solicitudes abiertas (sin dirección exacta).</summary>
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
    /// Abierto a cualquiera (incluso anónimo) para poder ver el detalle público de un post.
    /// La dirección exacta solo se incluye si quien llama es el dueño o el profesional aceptado.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _serviceService.GetByIdAsync(id, _currentUserService.UserId);
        return Ok(result);
    }
}
