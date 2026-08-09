// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/UpdateServiceAdminRequest.cs
//               — the body PUT /api/services/{id} expects from a MasterAdmin/Admin caller.
//               Backs the "Editar" form inside the "Ver Todos los Trabajos" CRUD panel on
//               Views/Dashboard/Admin.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class UpdateServiceAdminRequest
{
    public string? Description { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; }
    public string Zone { get; set; } = string.Empty;
}
