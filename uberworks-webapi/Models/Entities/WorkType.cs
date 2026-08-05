// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Representa una categoría de trabajo del catálogo administrable (ej.
//           "Plomería", "Electricidad"). Un Admin/MasterAdmin puede crear, editar o borrar
//           estas categorías desde la API sin que un desarrollador tenga que tocar código
//           (ver Controllers/WorkTypesController.cs).
// Entidades relacionadas: Service.cs (1:N — cada Service pertenece a un WorkType)
// Tablas relacionadas: TBL_WORKTYPES (mapeo en Data/Configurations/WorkTypeConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_WORKTYPES (categorías de trabajo).
/// </summary>
public class WorkType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }

    // Navegaciones
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
