// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/CancelReportRequest.cs —
//               the body POST /api/reports/{id}/cancel expects. Backs the "Borrar" action
//               inside the "Ver Todos los Reportes" CRUD panel on Views/Dashboard/Admin.cshtml
//               (reason is always required, per explicit request).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class CancelReportRequest
{
    public string Reason { get; set; } = string.Empty;
}
