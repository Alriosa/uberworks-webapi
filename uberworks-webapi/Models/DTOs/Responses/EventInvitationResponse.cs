// =====================================================================================
// FILE SUMMARY
// What it does: What the API returns for a professional's own Event invitation — carries
// the full Event detail alongside it (title/description/what's-not-included/date/location/
// company name) so the WebApp can render the whole "toda la descripción... fecha, lugar"
// screen from a single GET /api/events/invitations/mine call, no follow-up requests needed.
// Entities connected: EventInvitation.cs, Event.cs, User.cs (EventService.cs maps from there)
// Tables related: None directly — it's the "public shape" of a TBL_EVENT_INVITATIONS row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class EventInvitationResponse
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string EventDescription { get; set; } = string.Empty;
    public string? EventNotIncluded { get; set; }
    public DateTime EventDate { get; set; }
    public string EventLocation { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public EventInvitationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
