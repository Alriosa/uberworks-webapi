// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/UpdateReportRequest.cs —
//               the body PUT /api/reports/{id} expects. Backs the "Editar" form inside the
//               "Ver Todos los Reportes" CRUD panel on Views/Dashboard/Admin.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class UpdateReportRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ServiceId { get; set; }
    public int? ClientUserId { get; set; }
    public int? ProfessionalUserId { get; set; }
    public DateTime? IncidentDate { get; set; }
}
