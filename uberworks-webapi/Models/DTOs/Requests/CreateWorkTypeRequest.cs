// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de POST /api/worktypes — lo que un Admin/MasterAdmin manda
//           para crear una nueva categoría del catálogo (ej. "Plomería").
// Entidades relacionadas: WorkType.cs (indirectamente, vía WorkTypeService.CreateAsync)
// Tablas relacionadas: Ninguna directamente (TBL_WORKTYPES se llena desde WorkTypeService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CreateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }
}
