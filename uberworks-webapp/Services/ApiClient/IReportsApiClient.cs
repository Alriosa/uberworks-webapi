// =====================================================================================
// FILE SUMMARY
// What it does: Contract for talking to uberworks-webapi's /api/reports endpoints. Backs
//               both the Admin dashboard's report CRUD panel and (later) the Support
//               dashboard. CreateAsync takes plain IFormFile images (same reasoning as
//               IContactApiClient.SuggestServiceAsync) since filing a report can include
//               photos. Every method needs the caller's own JWT — every /api/reports
//               endpoint on the API side is [Authorize(Roles = "MasterAdmin,Admin,Support")].
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using Microsoft.AspNetCore.Http;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Services.ApiClient;

public interface IReportsApiClient
{
    Task<ReportResponse> CreateAsync(
        string accessToken,
        string title,
        string description,
        int? serviceId,
        int? clientUserId,
        int? professionalUserId,
        DateTime? incidentDate,
        List<IFormFile>? images);

    Task<List<ReportResponse>> GetAllAsync(string accessToken);
    Task<ReportResponse> GetByIdAsync(string accessToken, int id);
    Task<ReportResponse> UpdateAsync(string accessToken, int id, UpdateReportRequest request);

    /// <summary>Used both by the Admin dashboard's "Borrar" action and the Support dashboard's "Cancelar reporte" button.</summary>
    Task<ReportResponse> CancelAsync(string accessToken, int id, CancelReportRequest request);

    /// <summary>Support dashboard's "Resolver" button.</summary>
    Task<ReportResponse> ResolveAsync(string accessToken, int id, ResolveReportRequest request);

    /// <summary>Support dashboard's "Fallo a favor de nadie" button.</summary>
    Task<ReportResponse> NoFaultAsync(string accessToken, int id);
}
