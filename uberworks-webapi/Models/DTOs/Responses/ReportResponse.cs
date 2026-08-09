// =====================================================================================
// FILE SUMMARY
// What it does: What the API returns for a Report — full detail, including the
// human-readable names for every party involved (client/professional/who filed it/who
// resolved it) so the WebApp never has to make extra round-trips just to show names instead
// of raw ids. Images is the real List&lt;string&gt; shape of Report.ImagesJson (see
// ReportService for where that conversion happens) — nothing outside the Service layer ever
// sees the raw JSON. Backs both the Admin dashboard's report CRUD panel and the Support
// dashboard's report list/detail view.
// Entities connected: Report.cs, Service.cs, User.cs (ReportService.cs maps from there)
// Tables related: None directly — it's the "public shape" of a TBL_REPORTS row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

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
