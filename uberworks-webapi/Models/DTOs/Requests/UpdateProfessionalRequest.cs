// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of PUT /api/professionals/{id} — updates description,
//               experience, availability, and location. AverageRating is intentionally
//               absent: it isn't something the professional can edit manually, it will be
//               computed from Review.cs.
// Entities connected: Professional.cs (indirectly, via ProfessionalService.UpdateAsync)
// Tables related: None directly (TBL_PROFESSIONALS is updated from ProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateProfessionalRequest
{
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
