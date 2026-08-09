// =====================================================================================
// FILE SUMMARY
// What it does: Describes what the API returns when querying a Professional. Includes the
//               owning User's Username/FirstName/LastName flattened in here, so the client
//               (webapp/mobile) doesn't have to make a second call to /api/users/{id} just
//               to display the name. Email is intentionally NOT here: this response is
//               public (no login required to view a professional's profile), and there's
//               no legitimate reason to expose someone's contact email before any
//               relationship exists between them and a client. CompanyUserId is null
//               unless this worker was created by a Company (see ProfessionalService.
//               CreateByCompanyAsync).
// Entities connected: Professional.cs, User.cs (ProfessionalService.cs maps from there)
// Tables related: None directly — it's the combined "public shape" of TBL_PROFESSIONALS + TBL_USERS
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class ProfessionalResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Basic User data flattened in here, so the consumer doesn't need a second call.
    // No Email here on purpose — see FILE SUMMARY above.
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }

    /// <summary>Set only for workers created by a Company (see Professional.CompanyUserId).</summary>
    public int? CompanyUserId { get; set; }

    /// <summary>Relative URL (e.g. "/uploads/professional-photos/7-3f2c1e.jpg"), null until set.</summary>
    public string? PhotoUrl { get; set; }
}
