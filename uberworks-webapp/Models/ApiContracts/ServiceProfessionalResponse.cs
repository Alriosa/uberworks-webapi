// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/ServiceProfessionalResponse.cs
//               — one professional's proposal/negotiation on a Service, with their name and
//               rating flattened in. Returned by GET /api/services/{serviceId}/proposals
//               (Client-only — only the owning client can see who bid on their job).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

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
