// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/chats endpoints. Both
//               methods are keyed by serviceId — the caller never needs to know the other
//               party's raw professionalId/clientId (see IChatService.ResolvePartiesAsync on
//               the API side for how that's resolved and verified).
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IChatsApiClient
{
    Task<List<ChatMessageResponse>> GetConversationAsync(string accessToken, int serviceId);
    Task<ChatMessageResponse> SendMessageAsync(string accessToken, int serviceId, string message);
}
