// =====================================================================================
// FILE SUMMARY
// What it does: Describes exactly what fields uberworks-webapp must send in the body of
//               POST /api/users/external-login, after Google itself has already verified
//               the user's identity and handed the WebApp a verified email. This endpoint
//               is protected by RequireInternalSecretAttribute.cs (not JWT), since there
//               is no JWT yet — the whole point of the call is to obtain one.
// Entities connected: User.cs (UserService.ExternalLoginAsync converts this into a User,
//                      creating one automatically as Role=Client if the email is new)
// Tables related: None directly — only reaches TBL_USERS after passing through UserService.cs
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
