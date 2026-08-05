// =====================================================================================
// FILE SUMMARY
// What it does: Describes what the API returns when querying a proposal
//               (ServiceProfessional). Includes the professional's name and average rating
//               flattened in here, so the client can compare proposals without extra calls.
// Entities connected: ServiceProfessional.cs, Professional.cs, User.cs
//                      (ServiceProfessionalService.cs maps from there)
// Tables related: None directly — it's the combined "public shape" of
//                 TBL_SERVICE_PROFESSIONALS + TBL_PROFESSIONALS + TBL_USERS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class ServiceProfessionalResponse
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int ProfessionalId { get; set; }
    public string ProfessionalFirstName { get; set; } = string.Empty;
    public string ProfessionalLastName { get; set; } = string.Empty;
    public decimal ProfessionalAverageRating { get; set; }

    public decimal? NegotiatedPrice { get; set; }
    public int EstimatedArrivalMinutes { get; set; }
    public DateTime? ArrivalConfirmedAt { get; set; }
    public ServiceProfessionalStatus Status { get; set; }
}
