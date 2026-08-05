// =====================================================================================
// FILE SUMMARY
// What it does: Describes what the API returns when querying a Service. This is THE DTO
//               where the location privacy rule lives: Latitude/Longitude/ExactAddress are
//               nullable (?) and ServiceService.cs decides in real time whether to fill them
//               in or leave them null depending on who's asking (see Services/ServiceService.cs).
//               Zone always travels filled in, even in the public listing.
// Entities connected: Service.cs, WorkType.cs (ServiceService.cs maps from there)
// Tables related: None directly — it's the "public shape" (with privacy rules) of a
//                 TBL_SERVICES row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

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

    /// <summary>Always visible, even in the public listing of open services.</summary>
    public string Zone { get; set; } = string.Empty;

    /// <summary>
    /// Only filled in when the requester is the owning client or the professional whose
    /// proposal was accepted. In any other case they stay null.
    /// </summary>
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ExactAddress { get; set; }

    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }
}
