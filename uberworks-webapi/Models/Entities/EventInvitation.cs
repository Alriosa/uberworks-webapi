// =====================================================================================
// FILE SUMMARY
// What it does: One professional's invitation to one Event — this is the row that lets a
// Professional see "toda la descripción de que va a ser necesario y que no, fecha, lugar" and
// Accept/Decline (see EventService.RespondToInvitationAsync). Auto-created (one per
// professional linked to the company at the moment the Event is made — see
// EventService.CreateAsync), never created directly by a professional.
// Entities connected: Event.cs (N:1), User.cs (N:1, the Professional)
// Tables related: TBL_EVENT_INVITATIONS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_EVENT_INVITATIONS.
/// </summary>
public class EventInvitation
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int ProfessionalUserId { get; set; }
    public EventInvitationStatus Status { get; set; } = EventInvitationStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    // Navigation properties
    public Event? Event { get; set; }
    public User? Professional { get; set; }
}
