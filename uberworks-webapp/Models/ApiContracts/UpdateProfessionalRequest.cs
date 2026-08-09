// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/UpdateProfessionalRequest.cs
//               — the JSON body sent to PUT /api/professionals/{id}. AverageRating is
//               intentionally absent, same as the API side — it isn't editable by hand.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class UpdateProfessionalRequest
{
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
