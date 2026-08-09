// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/AdminServiceListItemResponse.cs
//               — one row per "Work Post", every status, full detail including the exact
//               location and the requesting client's username/full name. Returned by
//               GET /api/services. Backs the "Ver Todos los Trabajos" CRUD panel on
//               Views/Dashboard/Admin.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class AdminServiceListItemResponse
{
    public int Id { get; set; }
    public int WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientUsername { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ExactAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }
}
