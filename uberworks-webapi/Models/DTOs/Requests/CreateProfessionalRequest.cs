// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/professionals. It intentionally does NOT
//               include UserId: that value comes from the JWT of whoever is making the
//               request (via ICurrentUserService), never from what the client sends, so no
//               one can create a professional profile on someone else's behalf.
// Entities connected: Professional.cs (indirectly, via ProfessionalService.CreateAsync)
// Tables related: None directly (TBL_PROFESSIONALS is filled in from ProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

// UserId is intentionally NOT here: it's taken from the authenticated user (JWT), never
// from what the client sends in the body, so no one can create a profile on someone else's behalf.
public class CreateProfessionalRequest
{
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
