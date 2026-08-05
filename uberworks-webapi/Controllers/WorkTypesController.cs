// =====================================================================================
// FILE SUMMARY
// What it does: Exposes the full HTTP CRUD for the WorkType catalog. Still without real
//               [Authorize] (see the TODO below) — anyone can currently create/edit/delete
//               categories; this needs to be restricted to MasterAdmin/Admin.
// Entities connected: WorkType.cs (indirectly, via IWorkTypeService)
// Tables related: TBL_WORKTYPES (indirectly, through all the layers)
// =====================================================================================
using Microsoft.AspNetCore.Mvc;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Controllers;

// TODO: restrict Create/Update/Delete to [Authorize(Roles = "MasterAdmin,Admin")]
// (the catalog is managed by the app, not by any authenticated user).
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
