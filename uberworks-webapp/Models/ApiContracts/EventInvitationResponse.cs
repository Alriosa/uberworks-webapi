// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/EventInvitationResponse.cs
//               — a professional's own invitation, with the full Event detail attached.
//               Backs Views/Dashboard/EventInvitations.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

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
