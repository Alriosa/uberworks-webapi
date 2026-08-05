// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de PUT /api/worktypes/{id} — misma forma que
//           CreateWorkTypeRequest.cs, para editar una categoría existente.
// Entidades relacionadas: WorkType.cs (indirectamente, vía WorkTypeService.UpdateAsync)
// Tablas relacionadas: Ninguna directamente (TBL_WORKTYPES se actualiza desde
//                       WorkTypeService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }
}
