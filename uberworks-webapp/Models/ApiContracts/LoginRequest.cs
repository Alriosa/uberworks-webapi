// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/LoginRequest.cs — the JSON
//               shape sent to POST /api/users/login. Built by Services/ApiClient/UsersApiClient.cs
//               from the LoginViewModel the user fills out in Views/Account/Login.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
