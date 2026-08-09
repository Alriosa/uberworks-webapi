// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/chats/by-service/{serviceId} — the only
//               thing a caller ever supplies is the message text itself. Which conversation
//               it belongs to (the ProfessionalId/ClientId pair) and who's sending it
//               (SenderRole) are both resolved server-side from the route's serviceId and the
//               caller's own JWT — see ChatService.ResolvePartiesAsync — never trusted from
//               the request body.
// Entities connected: Chat.cs (indirectly, via ChatService.SendMessageByServiceAsync)
// Tables related: None directly (TBL_CHATS is filled in from ChatService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class SendChatMessageRequest
{
    public string Message { get; set; } = string.Empty;
}
