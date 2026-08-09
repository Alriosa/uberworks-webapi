// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/UpdateUserRequest.cs — the
//               JSON body sent to PUT /api/users/{id}. Only FirstName/LastName/Phone are
//               editable this way (not Email/Username/Password/Role — those need separate
//               flows), same restriction as the API side.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
