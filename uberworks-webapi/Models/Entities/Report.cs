// =====================================================================================
// FILE SUMMARY
// What it does: A dispute/incident report against a Service, handled from the Support
//               dashboard (Views/Dashboard/Support.cshtml) — and also listable/editable from
//               the Admin dashboard's "Ver Todos los Reportes" CRUD panel. ServiceId/
//               ClientUserId/ProfessionalUserId are all nullable because a report can be
//               filed manually (e.g. by an Admin, before a real "flag this job" flow exists
//               on the Client/Professional side — see this class's own backlog note) without
//               necessarily having every party on hand yet. ImagesJson stores a JSON array of
//               uploaded image URLs (same "local disk for now" plan as
//               Professional.PhotoUrl) — exposed as a real List&lt;string&gt; on
//               ReportResponse, never as raw JSON outside this layer.
//               Resolution fields (ResolutionMessage/PaymentOutcome/CancellationReason/
//               ResolvedByUserId/ResolvedAt) are all null until the report is closed one of
//               three ways — see ReportService.ResolveAsync/NoFaultAsync/CancelAsync.
// Entities connected: Service.cs (N:1, optional), User.cs (N:1 three times over: client,
//                      professional, the Support/Admin who created or resolved it)
// Tables related: TBL_REPORTS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_REPORTS.
/// </summary>
public class Report
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int? ServiceId { get; set; }
    public int? ClientUserId { get; set; }
    public int? ProfessionalUserId { get; set; }
    public int CreatedByUserId { get; set; }

    /// <summary>When the actual incident happened, if different from when it was filed.</summary>
    public DateTime? IncidentDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Open;

    /// <summary>Raw JSON array of image URLs — see ReportService for the real List&lt;string&gt; shape.</summary>
    public string? ImagesJson { get; set; }

    public string? ResolutionMessage { get; set; }
    public ReportPaymentOutcome? PaymentOutcome { get; set; }
    public string? CancellationReason { get; set; }
    public int? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public Service? Service { get; set; }
    public User? Client { get; set; }
    public User? Professional { get; set; }
}
