// =====================================================================================
// FILE SUMMARY
// What it does: Stores a single chat message between a client and a professional. Doesn't
//               have a Repository/Service/Controller built yet (nor real-time logic like
//               WebSockets/SignalR) — it's a pending piece.
// Entities connected: Professional.cs (N:1), User.cs (N:1, as Client)
// Tables related: TBL_CHATS (mapping in Data/Configurations/ChatConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_CHATS (a message between a client and a professional).
/// </summary>
public class Chat
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ClientId { get; set; }

    public string Message { get; set; } = string.Empty;
    public DateTime MessageDate { get; set; }

    // Navigation properties
    public Professional Professional { get; set; } = null!;
    public User Client { get; set; } = null!;
}
