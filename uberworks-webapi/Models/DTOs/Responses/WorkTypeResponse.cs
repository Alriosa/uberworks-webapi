// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe lo que la API devuelve al consultar un WorkType. Forma casi idéntica
//           a WorkType.cs porque es una entidad simple sin datos sensibles que ocultar.
// Entidades relacionadas: WorkType.cs (WorkTypeService.cs mapea de una a la otra)
// Tablas relacionadas: Ninguna directamente — es la "forma pública" de una fila de
//                       TBL_WORKTYPES
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class WorkTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }
}
