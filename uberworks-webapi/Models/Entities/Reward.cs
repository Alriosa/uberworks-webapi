// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Guarda el saldo de puntos de recompensa de un usuario (1 fila por usuario,
//           que se actualiza con el tiempo — no un historial de eventos). Todavía no
//           tiene Repository/Service/Controller construidos.
// Entidades relacionadas: User.cs (1:1)
// Tablas relacionadas: TBL_REWARDS (mapeo en Data/Configurations/RewardConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_REWARDS.
/// </summary>
public class Reward
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public int Points { get; set; }
    public DateTime LastUpdateDate { get; set; }

    // Navegaciones
    public User User { get; set; } = null!;
}
