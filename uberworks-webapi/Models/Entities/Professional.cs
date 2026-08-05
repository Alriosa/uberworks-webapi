// =====================================================================================
// FILE SUMMARY
// What it does: Represents the "professional profile" that a User with Role=Professional
//               can create (1:1 relationship — each Professional belongs to exactly one
//               User). Stores description, experience, availability, location, and average
//               rating (which will be computed from Review.cs later on).
// Entities connected: User.cs (1:1, profile owner), ServiceProfessional.cs (1:N, its
//                      proposals to different Services), Review.cs (1:N), Chat.cs (1:N)
// Tables related: TBL_PROFESSIONALS (mapping in Data/Configurations/ProfessionalConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_PROFESSIONALS. Extends User in a 1:1 relationship.
/// </summary>
public class Professional
{
    public int Id { get; set; }

    /// <summary>
    /// 1:1 FK to User (PK_USER_ID in the diagram).
    /// </summary>
    public int UserId { get; set; }

    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<ServiceProfessional> ServiceProfessionals { get; set; } = new List<ServiceProfessional>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
