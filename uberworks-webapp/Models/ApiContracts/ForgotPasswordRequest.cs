// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/ForgotPasswordRequest.cs —
//               the JSON shape sent to POST /api/users/forgot-password.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
