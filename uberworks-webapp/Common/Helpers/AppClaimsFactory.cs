// =====================================================================================
// FILE SUMMARY
// What it does: Builds the ClaimsIdentity used for the WebApp's login cookie, from an
//               AuthResponse the API returned (whether that came from a normal
//               Email/Password login in AccountController.cs, or from a Google/Facebook
//               sign-in in Program.cs's AddGoogle()/AddFacebook().Events.OnCreatingTicket).
//               Kept in one place so every sign-in flow produces the exact same set of
//               claims — most importantly the "access_token" claim carrying the API's raw
//               JWT (which later Controllers attach as "Authorization: Bearer {token}" when
//               calling protected endpoints), and "requires_password_setup", which
//               _SetPasswordModal.cshtml checks on every authenticated page to decide
//               whether to show the "create your password" modal. Because this claim is
//               (re)computed fresh from the API's AuthResponse on every sign-in, if someone's
//               password-creation attempt was ever interrupted (e.g. a power outage) the
//               modal simply reappears the next time they log in — the API is still the
//               source of truth, not anything remembered client-side.
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
            new("access_token", auth.Token),
            new("requires_password_setup", auth.RequiresPasswordSetup ? "true" : "false")
        };

        return new ClaimsIdentity(claims, authenticationType);
    }
}
