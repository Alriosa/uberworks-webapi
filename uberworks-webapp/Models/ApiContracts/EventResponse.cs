// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/EventResponse.cs — a
//               Company's "Crear Evento" call for professionals, plus a live tally of how
//               its invitations have been answered. Backs the Company/Manager dashboard's
//               "Crear Evento"/events list panel.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

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
