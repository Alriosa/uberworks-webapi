// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/CreateServiceProfessionalRequest.cs
//               — the body POST /api/services/{serviceId}/proposals expects. This is the
//               "cuadro especial donde el profesional dictará el monto" the user asked for:
//               a professional proposes their own price (and estimated arrival time) on a
//               job offer, instead of the price being fixed upfront.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class CreateServiceProfessionalRequest
{
    public decimal? NegotiatedPrice { get; set; }
    public int EstimatedArrivalMinutes { get; set; }
}
