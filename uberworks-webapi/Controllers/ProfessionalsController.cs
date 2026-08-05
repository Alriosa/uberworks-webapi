// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Expone los endpoints HTTP de Professional. Recibe la petición, la valida a
//           nivel de forma ([Authorize] revisa el rol antes de que el código entre al
//           método), y delega TODA la lógica real a IProfessionalService — el Controller
//           nunca decide reglas de negocio, solo traduce HTTP ↔ llamadas a Services.
// Entidades relacionadas: Professional.cs (indirectamente, vía IProfessionalService)
// Tablas relacionadas: TBL_PROFESSIONALS (indirectamente, a través de todas las capas)
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
}
