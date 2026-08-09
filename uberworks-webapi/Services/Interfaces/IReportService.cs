// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the dispute/report business logic. ReportsController.cs
// depends on this interface. CreateAsync/UpdateAsync back the Admin dashboard's report CRUD
// panel; ResolveAsync/NoFaultAsync/CancelAsync back the Support dashboard's 3 action buttons
// (also reused by the Admin CRUD panel's "Borrar" action, which just calls CancelAsync).
// CreateFromClientAsync backs the Client dashboard's "Contactar con Soporte" self-service
// view, per explicit request — unlike CreateAsync (Admin/Support filing on someone else's
// behalf), ClientUserId here is ALWAYS the caller, never user-supplied, and an optional
// ServiceId is verified to actually belong to that caller before the report is filed.
// imageUrls always arrives as already-saved relative URLs (ReportsController.cs does the
// actual file I/O, same split as ProfessionalsController.UploadPhoto) — this layer only
// ever deals with strings.
// Entities connected: Report.cs
// Tables related: TBL_REPORTS (indirectly, via ReportService.cs)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IReportService
{
    Task<ReportResponse> CreateAsync(int createdByUserId, CreateReportRequest request, IReadOnlyList<string> imageUrls);

    /// <summary>Client's own "Contactar con Soporte" self-service report.</summary>
    Task<ReportResponse> CreateFromClientAsync(int clientUserId, ClientCreateReportRequest request, IReadOnlyList<string> imageUrls);

    Task<IReadOnlyList<ReportResponse>> GetAllAsync();
    Task<ReportResponse> GetByIdAsync(int id);
    Task<ReportResponse> UpdateAsync(int id, UpdateReportRequest request);

    /// <summary>
    /// "Resolver": records the resolution message and the held-payment decision, closes the
    /// report as Resolved. See ResolveReportRequest.cs for the honest note on what's real
    /// (persisted on the report) vs. not yet wired up (an actual live chat message, an
    /// actual Payment release/refund — Chat.cs/Payment.cs have no Repository/Service yet).
    /// </summary>
    Task<ReportResponse> ResolveAsync(int id, int resolvedByUserId, ResolveReportRequest request);

    /// <summary>
    /// "Fallo a favor de nadie": closes the report as Resolved with no payment side taken —
    /// the hold is simply lifted and the normal payment flow proceeds (see PaymentStatus.cs).
    /// </summary>
    Task<ReportResponse> NoFaultAsync(int id, int resolvedByUserId);

    /// <summary>"Cancelar reporte": always requires a reason. Sets Status=Cancelled.</summary>
    Task<ReportResponse> CancelAsync(int id, int resolvedByUserId, CancelReportRequest request);
}
