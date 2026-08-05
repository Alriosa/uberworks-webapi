// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/services — what a client sends when
//               creating a "Work Post". Includes a WhatsApp-style "share location"
//               (Latitude/Longitude + ExactAddress) plus the public Zone. ClientId is NOT
//               here: it comes from the JWT, same as in CreateProfessionalRequest.cs.
// Entities connected: Service.cs (indirectly, via ServiceService.CreateAsync)
// Tables related: None directly (TBL_SERVICES is filled in from ServiceService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CreateServiceRequest
{
    public int WorkTypeId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }

    // "Share location" style GPS data. ExactAddress and the coordinates are never shown
    // publicly — only Zone is visible before a proposal is accepted.
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ExactAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}
