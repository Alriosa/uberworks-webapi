// =====================================================================================
// FILE SUMMARY
// What it does: This is the "Work Post" — what a client publishes when they need a service
//               (e.g. "I need my fridge fixed"). Stores the exact GPS location and address
//               (private, only visible to the owner and the accepted professional), the
//               public Zone (e.g. "Downtown", visible to everyone), and the job-closing data
//               (completion photo + confirmation from both parties). All the "who can see
//               what" logic lives in Services/ServiceService.cs, not here — this class only
//               describes the shape of the data.
// Entities connected: WorkType.cs (N:1), User.cs (N:1, as Client),
//                      ServiceProfessional.cs (1:N, the proposals it receives),
//                      Review.cs (1:N), Payment.cs (1:N)
// Tables related: TBL_SERVICES (mapping in Data/Configurations/ServiceConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_SERVICES (a service request created by a client).
/// </summary>
public class Service
{
    public int Id { get; set; }
    public int WorkTypeId { get; set; }
    public int ClientId { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
    public DateTime RequestDate { get; set; }

    // Location: the exact address is private, only exposed to the owner (client)
    // and to the professional whose proposal was accepted. The Zone is public from the start.
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ExactAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;

    // Job closing: evidence + mutual confirmation.
    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }

    // Navigation properties
    public WorkType WorkType { get; set; } = null!;
    public User Client { get; set; } = null!;
    public ICollection<ServiceProfessional> ServiceProfessionals { get; set; } = new List<ServiceProfessional>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
