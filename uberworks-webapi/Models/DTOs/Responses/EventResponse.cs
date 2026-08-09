// =====================================================================================
// FILE SUMMARY
// What it does: What the API returns for an Event, including a real-time tally of how its
// invitations have been answered (AcceptedCount/DeclinedCount/PendingCount) so the Company/
// Manager dashboard can show "7 de 10 confirmados" without a separate call. Backs
// GET /api/events/mine.
// Entities connected: Event.cs, EventInvitation.cs (EventService.cs maps from there)
// Tables related: None directly — it's the "public shape" of a TBL_EVENTS row
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class EventResponse
{
    public int Id { get; set; }
    public int CompanyUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? NotIncluded { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int ProfessionalsNeededCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public int TotalInvited { get; set; }
    public int AcceptedCount { get; set; }
    public int DeclinedCount { get; set; }
    public int PendingCount { get; set; }
}
