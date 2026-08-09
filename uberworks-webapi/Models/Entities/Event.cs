// =====================================================================================
// FILE SUMMARY
// What it does: A Company's call for professionals — e.g. "necesito 10 cocineros para un
//               evento el sábado". Only a Company can create one (never a Manager — the one
//               explicit difference between the Company and Manager dashboards). Creating an
//               Event auto-generates one EventInvitation per Professional currently linked
//               to that company (see EventService.CreateAsync) — Description/NotIncluded
//               together are the "toda la descripción de que va a ser necesario y que no"
//               the invited professionals see.
// Entities connected: User.cs (N:1, the Company), EventInvitation.cs (1:N)
// Tables related: TBL_EVENTS
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_EVENTS.
/// </summary>
public class Event
{
    public int Id { get; set; }
    public int CompanyUserId { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>What WILL be needed from the professional at this event.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>What will NOT be needed/included — the other half of the "qué sí y qué no" ask.</summary>
    public string? NotIncluded { get; set; }

    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int ProfessionalsNeededCount { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User? Company { get; set; }
    public ICollection<EventInvitation> Invitations { get; set; } = new List<EventInvitation>();
}
