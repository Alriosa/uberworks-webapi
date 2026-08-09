// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the Chat business logic. ChatsController.cs depends on this
//               interface. Both methods are keyed by serviceId rather than a raw
//               professionalId/clientId pair — a caller never has to know or supply the other
//               party's internal id; it's resolved from the Service's accepted proposal and
//               verified against the caller's own identity (see ChatService.
//               ResolvePartiesAsync for the ownership checks).
// Entities connected: Chat.cs
// Tables related: TBL_CHATS (indirectly, via ChatService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IChatService
{
    /// <summary>Only the Service's own client or its accepted professional can read this.</summary>
    Task<IReadOnlyList<ChatMessageResponse>> GetConversationByServiceAsync(int serviceId, int callerUserId, UserRole callerRole);

    /// <summary>Only the Service's own client or its accepted professional can send here.</summary>
    Task<ChatMessageResponse> SendMessageByServiceAsync(int serviceId, int callerUserId, UserRole callerRole, SendChatMessageRequest request);
}
