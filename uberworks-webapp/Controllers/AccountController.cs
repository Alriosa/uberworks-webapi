// =====================================================================================
// FILE SUMMARY
// What it does: Handles Login/Register/Logout for the WebApp. On successful login, calls
//               the API via IUsersApiClient, then signs the browser in with a cookie
//               (Cookie Authentication, configured in Program.cs) carrying the user's
//               identity AND the raw JWT as a claim ("access_token") — that JWT is what
//               future Controllers will attach as "Authorization: Bearer {token}" when
//               calling protected API endpoints on the user's behalf. The Controller only
//               handles HTTP/form concerns; all the actual API communication goes through
//               IUsersApiClient, never directly through HttpClient here. GoogleLogin/
//               GoogleCallback and FacebookLogin/FacebookCallback back the "Continue with
//               Google"/"Continue with Facebook" buttons: by the time the *Callback action
//               runs, Program.cs's AddGoogle()/AddFacebook().Events.OnCreatingTicket has
//               already exchanged the provider-verified email for the API's JWT and signed
//               the cookie in — these actions only have to redirect. SetPassword backs the
//               "create your password" modal (_SetPasswordModal.cshtml), shown to anyone
//               signed in via Google/Facebook who hasn't set a real password yet — on
//               success, it re-issues the auth cookie with "requires_password_setup" flipped
//               to false so the modal stops appearing for the REST OF THIS SESSION; if the
//               browser closes mid-attempt (or the API call itself fails), the next sign-in
//               re-fetches the flag fresh from the API and the modal simply reappears.
//               ForgotPassword/ResetPassword back the "forgot password" email flow:
//               ForgotPassword always shows the same generic success message (matching the
//               API's own behavior — never reveals whether an email exists), and
//               ResetPassword reads the token from the query string (the email link) and
//               carries it through the form as a hidden field. Login/GoogleCallback/
//               FacebookCallback redirect to Dashboard/LandingPage (not Home/LandingPage)
//               after a successful sign-in when there's no returnUrl — DashboardController.cs
//               picks the right view for the user's role.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS only indirectly, through the API
// =====================================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uberworks_webapp.Common;
using uberworks_webapp.Common.Exceptions;
using uberworks_webapp.Common.Helpers;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Models.ViewModels;
using uberworks_webapp.Services.ApiClient;

namespace uberworks_webapp.Controllers;

public class AccountController : Controller
{
    private readonly IUsersApiClient _usersApiClient;
    private readonly GoogleLoginOptions _googleLoginOptions;
    private readonly FacebookLoginOptions _facebookLoginOptions;

    public AccountController(IUsersApiClient usersApiClient, GoogleLoginOptions googleLoginOptions, FacebookLoginOptions facebookLoginOptions)
    {
        _usersApiClient = usersApiClient;
        _googleLoginOptions = googleLoginOptions;
        _facebookLoginOptions = facebookLoginOptions;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null, string? error = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["GoogleLoginEnabled"] = _googleLoginOptions.IsEnabled;
        ViewData["FacebookLoginEnabled"] = _facebookLoginOptions.IsEnabled;
        if (!string.IsNullOrEmpty(error))
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var auth = await _usersApiClient.LoginAsync(new LoginRequest
            {
                Email = model.Email,
                Password = model.Password
            });

            await SignInAsync(auth);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("LandingPage", "Dashboard");
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewData["GoogleLoginEnabled"] = _googleLoginOptions.IsEnabled;
        ViewData["FacebookLoginEnabled"] = _facebookLoginOptions.IsEnabled;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _usersApiClient.RegisterAsync(new RegisterUserRequest
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password
            });

            TempData["SuccessMessage"] = "Account created successfully. You can now log in.";
            return RedirectToAction(nameof(Login));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        if (!_googleLoginOptions.IsEnabled)
        {
            // GoogleAuth:ClientId/ClientSecret aren't configured yet — Program.cs never
            // registered the Google scheme, so Challenge() would throw. Fail politely
            // instead of a 500.
            return NotFound("Google sign-in is not configured yet.");
        }

        var redirectUrl = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult GoogleCallback(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("LandingPage", "Dashboard");
    }

    [HttpGet]
    public IActionResult FacebookLogin(string? returnUrl = null)
    {
        if (!_facebookLoginOptions.IsEnabled)
        {
            // Same reasoning as GoogleLogin above — FacebookAuth:AppId/AppSecret aren't
            // configured yet, so Program.cs never registered the Facebook scheme.
            return NotFound("Facebook sign-in is not configured yet.");
        }

        var redirectUrl = Url.Action(nameof(FacebookCallback), "Account", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult FacebookCallback(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("LandingPage", "Dashboard");
    }

    /// <summary>
    /// Lets a Google/Facebook-created account (User.IsPasswordSet == false, surfaced here as
    /// the "requires_password_setup" cookie claim — see AppClaimsFactory.cs) set a real
    /// password. Re-issues the auth cookie on success so the modal stops appearing for the
    /// rest of this session; if it never completes (browser closed, API call failed), the
    /// next sign-in re-fetches the flag from the API and the modal reappears — nothing about
    /// "did they finish setting a password" is trusted to the client.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model, string? returnUrl = null)
    {
        IActionResult redirectResult = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction("LandingPage", "Dashboard");

        if (!ModelState.IsValid)
        {
            TempData["SetPasswordError"] = "Please enter a password of at least 8 characters, and make sure both fields match.";
            return redirectResult;
        }

        var accessToken = User.FindFirst("access_token")?.Value;
        if (string.IsNullOrEmpty(accessToken))
        {
            // Shouldn't happen for an authenticated user, but fail safely instead of a 500.
            TempData["SetPasswordError"] = "Your session has expired. Please log in again.";
            return RedirectToAction(nameof(Login));
        }

        try
        {
            await _usersApiClient.SetPasswordAsync(accessToken, model.NewPassword);

            var identity = (ClaimsIdentity)User.Identity!;
            var oldClaim = identity.FindFirst("requires_password_setup");
            if (oldClaim is not null)
            {
                identity.RemoveClaim(oldClaim);
            }
            identity.AddClaim(new Claim("requires_password_setup", "false"));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            TempData["SetPasswordSuccess"] = "Your password has been created. You can now log in with it too.";
        }
        catch (ApiException ex)
        {
            TempData["SetPasswordError"] = ex.Message;
        }

        return redirectResult;
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Always the same message whether or not the email exists — matches the API's own
        // ForgotPasswordAsync behavior (see its FILE SUMMARY).
        await _usersApiClient.ForgotPasswordAsync(new ForgotPasswordRequest { Email = model.Email });

        ViewData["SubmittedMessage"] = "If that email exists in our system, a password reset link has been sent to it.";
        return View(new ForgotPasswordViewModel());
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _usersApiClient.ResetPasswordAsync(new ResetPasswordRequest
            {
                Token = model.Token,
                NewPassword = model.NewPassword
            });

            TempData["SuccessMessage"] = "Your password has been reset. You can now log in.";
            return RedirectToAction(nameof(Login));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("LandingPage", "Home");
    }

    private async Task SignInAsync(AuthResponse auth)
    {
        var identity = AppClaimsFactory.CreateIdentity(auth, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
