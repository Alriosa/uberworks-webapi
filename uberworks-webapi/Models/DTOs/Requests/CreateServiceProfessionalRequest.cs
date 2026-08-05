// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/services/{serviceId}/proposals — what a
//               professional sends when bidding on a Service (negotiated price + estimated
//               arrival minutes). ServiceId comes from the route and ProfessionalId from
//               the JWT — neither is in this DTO.
// Entities connected: ServiceProfessional.cs (indirectly, via
//                      ServiceProfessionalService.CreateProposalAsync)
// Tables related: None directly (TBL_SERVICE_PROFESSIONALS is filled in from
//                 ServiceProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

// ServiceId comes from the route, and ProfessionalId is resolved from the authenticated user (JWT).
public class CreateServiceProfessionalRequest
{
    public decimal? NegotiatedPrice { get; set; }
    public int EstimatedArrivalMinutes { get; set; }
}
