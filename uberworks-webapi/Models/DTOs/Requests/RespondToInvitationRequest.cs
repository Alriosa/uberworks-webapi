// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/events/invitations/{id}/respond — a
// professional accepting or declining an Event invitation.
// Entities connected: EventInvitation.cs (indirectly, via EventService.RespondToInvitationAsync)
// Tables related: None directly (TBL_EVENT_INVITATIONS is updated from EventService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class RespondToInvitationRequest
{
    public bool Accept { get; set; }
}
