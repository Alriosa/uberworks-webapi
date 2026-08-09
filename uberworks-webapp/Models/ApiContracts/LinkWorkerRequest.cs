// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/LinkWorkerRequest.cs — the
//               body POST /api/professionals/link-existing expects. Backs the "invitar
//               profesional" search box inside the Company/Manager dashboard's "Ver Todos
//               mis Profesionales" panel.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class LinkWorkerRequest
{
    public string Contact { get; set; } = string.Empty;
}
