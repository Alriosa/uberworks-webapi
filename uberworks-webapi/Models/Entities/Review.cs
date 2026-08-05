// =====================================================================================
// FILE SUMMARY
// What it does: Stores the mutual rating after a Service — the client rates the
//               professional and vice versa (1 to 5), plus a comment. Doesn't have a
//               Repository/Service/Controller built yet (one of the last pending pieces of
//               a Service's lifecycle).
// Entities connected: Professional.cs (N:1), Service.cs (N:1), User.cs (N:1, as Client)
// Tables related: TBL_REVIEWS (mapping in Data/Configurations/ReviewConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_REVIEWS.
/// </summary>
public class Review
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ServiceId { get; set; }
    public int ClientId { get; set; }

    /// <summary>Rating the client gives the professional (1-5).</summary>
    public byte? ClientRating { get; set; }

    /// <summary>Rating the professional gives the client (1-5).</summary>
    public byte? ProfessionalRating { get; set; }

    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; }

    // Navigation properties
    public Professional Professional { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public User Client { get; set; } = null!;
}
