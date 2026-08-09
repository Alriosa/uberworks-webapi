// =====================================================================================
// FILE SUMMARY
// What it does: Stores a single chat message between a client and a professional.
//               Repositories/ChatRepository.cs, Services/ChatService.cs, and
//               Controllers/ChatsController.cs implement the real send/read flow — per
//               explicit request ("iniciar chat con la persona cliente, para hablar sobre el
//               trabajo"). A conversation is keyed by the (ProfessionalId, ClientId) pair,
//               not by a specific Service — TBL_CHATS has no ServiceId column, so two people
//               who've worked together on multiple jobs share one continuous message
//               history. ChatService resolves WHICH (ProfessionalId, ClientId) pair to use
//               from a given Service's accepted proposal (see ChatService.ResolvePartiesAsync)
//               so callers never have to know a raw ProfessionalId themselves. SenderRole
//               (added by AddSenderRoleToChats.sql) is what makes a real two-sided
//               conversation possible — without it there was no way to tell which of the two
//               parties sent a given message. No real-time logic yet (WebSockets/SignalR) —
//               the WebApp polls/reloads instead.
// Entities connected: Professional.cs (N:1), User.cs (N:1, as Client)
// Tables related: TBL_CHATS (mapping in Repositories/ChatRepository.cs — this app uses
//                 Dapper with raw SQL, not EF Core, so there's no Configurations file)
// =====================================================================================
using uberworks_webapi.Common.Enums;

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

    /// <summary>Which of the two parties sent THIS message.</summary>
    public ChatSenderRole SenderRole { get; set; }

    // Navigation properties
    public Professional Professional { get; set; } = null!;
    public User Client { get; set; } = null!;
}
