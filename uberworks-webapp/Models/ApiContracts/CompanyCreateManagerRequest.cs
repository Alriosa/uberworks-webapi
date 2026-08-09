// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/CompanyCreateManagerRequest.cs
//               — the body POST /api/users/company-create-manager expects. Backs the
//               Company dashboard's "Crear Manager" form (Company-only — see
//               Views/Dashboard/Company.cshtml). No Password field — the new Manager gets a
//               "set your password" email instead.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class CompanyCreateManagerRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
