// =====================================================================================
// FILE SUMMARY
// What it does: Describes what the API returns when querying a Professional. Includes the
//               owning User's FirstName/LastName/Email flattened in here, so the client
//               (webapp/mobile) doesn't have to make a second call to /api/users/{id} just
//               to display the name.
// Entities connected: Professional.cs, User.cs (ProfessionalService.cs maps from there)
// Tables related: None directly — it's the combined "public shape" of TBL_PROFESSIONALS + TBL_USERS
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class ProfessionalResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Basic User data flattened in here, so the consumer doesn't need a second call.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }
}
