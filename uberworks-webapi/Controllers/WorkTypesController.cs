// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Expone el CRUD HTTP completo del catálogo de WorkTypes. Todavía sin
//           [Authorize] real (ver el TODO abajo) — cualquiera puede crear/editar/borrar
//           categorías por ahora; falta restringirlo a MasterAdmin/Admin.
// Entidades relacionadas: WorkType.cs (indirectamente, vía IWorkTypeService)
// Tablas relacionadas: TBL_WORKTYPES (indirectamente, a través de todas las capas)
// =====================================================================================
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

// TODO: restringir Create/Update/Delete a [Authorize(Roles = "MasterAdmin,Admin")]
// (el catálogo lo administra la app, no cualquier usuario autenticado).
[ApiController]
[Route("api/[controller]")]
public class WorkTypesController : ControllerBase
{
    private readonly IWorkTypeService _workTypeService;

    public WorkTypesController(IWorkTypeService workTypeService)
    {
        _workTypeService = workTypeService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkTypeRequest request)
    {
        var result = await _workTypeService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workTypeService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _workTypeService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkTypeRequest request)
    {
        var result = await _workTypeService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workTypeService.DeleteAsync(id);
        return NoContent();
    }
}
