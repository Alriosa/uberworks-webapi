// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for Chat. A conversation is identified by the
//               (professionalId, clientId) pair — see Chat.cs's FILE SUMMARY for why there's
//               no ServiceId involved. GetConversationAsync returns every message between
//               that one pair, oldest first (a real message thread). AddAsync appends one.
// Entities connected: Chat.cs
// Tables related: TBL_CHATS (indirectly, via ChatRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IChatRepository
{
    Task<IReadOnlyList<Chat>> GetConversationAsync(int professionalId, int clientId);
    Task AddAsync(Chat chat);
}
