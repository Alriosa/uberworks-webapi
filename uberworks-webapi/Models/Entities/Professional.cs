// =====================================================================================
// FILE SUMMARY
// What it does: Represents the "professional profile" that a User with Role=Professional
//               can create (1:1 relationship — each Professional belongs to exactly one
//               User). Stores description, experience, availability, location, and average
//               rating (which will be computed from Review.cs later on). CompanyUserId is
//               optional: it's set only for "workers" created by a Company account (see
//               ProfessionalService.CreateByCompanyAsync) — a Professional created directly
//               by a Manager/Admin/MasterAdmin, or one that registered itself in the future,
//               has CompanyUserId = null and isn't managed by anyone.
// Entities connected: User.cs (1:1, profile owner; also 1:N as the owning Company, via
//                      CompanyUserId), ServiceProfessional.cs (1:N, its proposals to
//                      different Services), Review.cs (1:N), Chat.cs (1:N)
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

    public string Description { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string Availability { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Relative URL to the professional's profile photo (e.g. "/uploads/professional-photos/
    /// 7-3f2c1e.jpg"), set via POST /api/professionals/{id}/photo. Null until they upload one.
    /// Stored on local disk under wwwroot/uploads/professional-photos for now — see
    /// ProfessionalsController.UploadPhoto's FILE SUMMARY for the plan to move this to
    /// external storage later.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Optional FK to the User (Role=Company) that created this worker. Null for
    /// Professionals not managed by any company.
    /// </summary>
    public int? CompanyUserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public User? CompanyUser { get; set; }
    public ICollection<ServiceProfessional> ServiceProfessionals { get; set; } = new List<ServiceProfessional>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
