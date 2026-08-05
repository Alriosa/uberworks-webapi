// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Guarda una sanción aplicada a un usuario (temporal o permanente) con su razón
//           y fechas de inicio/fin. Todavía no tiene Repository/Service/Controller
//           construidos — probablemente se conecte con los permisos de Admin que
//           discutiremos más adelante (un Admin podría penalizar a un usuario).
// Entidades relacionadas: User.cs (N:1)
// Tablas relacionadas: TBL_PENALTIES (mapeo en Data/Configurations/PenaltyConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_PENALTIES.
/// </summary>
public class Penalty
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public PenaltyType? Type { get; set; }
    public string? Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navegaciones
    public User User { get; set; } = null!;
}
