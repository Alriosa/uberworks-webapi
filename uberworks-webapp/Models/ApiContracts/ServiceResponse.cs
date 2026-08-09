// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/ServiceResponse.cs — a
//               "Work Post" as the API returns it, from GET /api/services/open (public
//               listing, used as the Professional dashboard's job offers) and
//               GET /api/services/mine (a Client's own request history). Latitude/Longitude/
//               ExactAddress stay null unless the caller is the owning client or the
//               accepted professional — same privacy rule as the API side.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ServiceResponse
{
    public int Id { get; set; }
    public int WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public string Zone { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ExactAddress { get; set; }
    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }
}
