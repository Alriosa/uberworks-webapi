// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/events endpoints. Backs the
//               Company/Manager dashboard's event panel and the Professional-facing
//               EventInvitations.cshtml. Every method needs the caller's own JWT.
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IEventsApiClient
{
    Task<EventResponse> CreateAsync(string accessToken, CreateEventRequest request);
    Task<List<EventResponse>> GetMyEventsAsync(string accessToken);
    Task<List<EventInvitationResponse>> GetMyInvitationsAsync(string accessToken);
    Task<EventInvitationResponse> RespondToInvitationAsync(string accessToken, int invitationId, bool accept);
}
