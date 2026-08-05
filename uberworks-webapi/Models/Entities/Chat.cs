// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Guarda un mensaje individual del chat entre un cliente y un profesional.
//           Todavía no tiene Repository/Service/Controller construidos (ni lógica de
//           tiempo real tipo WebSockets/SignalR) — es una pieza pendiente.
// Entidades relacionadas: Professional.cs (N:1), User.cs (N:1, como Client)
// Tablas relacionadas: TBL_CHATS (mapeo en Data/Configurations/ChatConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_CHATS (mensaje entre un cliente y un profesional).
/// </summary>
public class Chat
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ClientId { get; set; }

    public string? Message { get; set; }
    public DateTime MessageDate { get; set; }

    // Navegaciones
    public Professional Professional { get; set; } = null!;
    public User Client { get; set; } = null!;
}
