// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of PUT /api/users/{id} — only allows changing first
//               name, last name, and phone (not email, password, or role — those require
//               separate flows).
// Entities connected: User.cs (indirectly, via UserService.UpdateAsync)
// Tables related: None directly (TBL_USERS is updated from UserService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
