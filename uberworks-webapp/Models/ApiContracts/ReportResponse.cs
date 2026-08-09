// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/ReportResponse.cs — full
//               detail of a dispute/incident report, including human-readable names for
//               every party involved. Backs the "Ver Todos los Reportes" panel on
//               Views/Dashboard/Admin.cshtml and (later) the Support dashboard.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ReportResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int? ServiceId { get; set; }
    public string? ServiceDescription { get; set; }

    public int? ClientUserId { get; set; }
    public string? ClientUsername { get; set; }

    public int? ProfessionalUserId { get; set; }
    public string? ProfessionalUsername { get; set; }

    public int CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; } = string.Empty;

    public DateTime? IncidentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReportStatus Status { get; set; }
    public List<string> Images { get; set; } = new();

    public string? ResolutionMessage { get; set; }
    public ReportPaymentOutcome? PaymentOutcome { get; set; }
    public string? CancellationReason { get; set; }
    public int? ResolvedByUserId { get; set; }
    public string? ResolvedByUsername { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
