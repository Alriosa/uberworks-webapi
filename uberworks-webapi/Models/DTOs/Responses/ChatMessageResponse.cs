// =====================================================================================
// FILE SUMMARY
// What it does: What the API returns for a Chat message — includes SenderRole (so the
//               WebApp can align "yours" vs "theirs" in the conversation UI) and
//               SenderUsername (so the WebApp never has to make an extra round-trip just to
//               show a name instead of a raw id).
// Entities connected: Chat.cs, User.cs, Professional.cs (ChatService.cs maps from there)
// Tables related: None directly — it's the "public shape" of a TBL_CHATS row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class ChatMessageResponse
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ClientId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime MessageDate { get; set; }
    public ChatSenderRole SenderRole { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
}
