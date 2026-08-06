// =====================================================================================
// FILE SUMMARY
// What it does: Builds the ClaimsIdentity used for the WebApp's login cookie, from an
//               AuthResponse the API returned (whether that came from a normal
//               Email/Password login in AccountController.cs, or from a Google sign-in in
//               Program.cs's AddGoogle().Events.OnCreatingTicket). Kept in one place so
//               both flows produce the exact same set of claims — most importantly the
//               "access_token" claim carrying the API's raw JWT, which later Controllers
//               attach as "Authorization: Bearer {token}" when calling protected endpoints.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
using System.Security.Claims;
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Common.Helpers;

public static class AppClaimsFactory
{
    public static ClaimsIdentity CreateIdentity(AuthResponse auth, string authenticationType)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.User.Id.ToString()),
            new(ClaimTypes.Name, auth.User.Username),
            new(ClaimTypes.Email, auth.User.Email),
            new(ClaimTypes.Role, auth.User.Role.ToString()),
            new("access_token", auth.Token)
        };

        return new ClaimsIdentity(claims, authenticationType);
    }
}
