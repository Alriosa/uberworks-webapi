// =====================================================================================
// FILE SUMMARY
// What it does: Entry point of the WebApp. Registers MVC (Controllers + Views), a typed
//               HttpClient for talking to uberworks-webapi (base URL from
//               appsettings.json → Api:BaseUrl, with "X-Client-Source: WebApp" AND
//               "X-Internal-Secret" attached to every outgoing request — the first lets the
//               API's audit logs tell this traffic apart from Mobile or direct calls, the
//               second is what POST /api/users/external-login checks via
//               RequireInternalSecretAttribute.cs on the API side, since that one call has
//               no JWT yet — the same two typed clients also back CompanyController.cs's
//               calls to /api/professionals/company-create and /api/professionals/my-workers),
//               cookie-based authentication (so once a user logs in via
//               AccountController, their identity — and the API's JWT — persists across page
//               requests without the browser having to resend credentials each time), and
//               Google AND Facebook as additional "external" sign-in schemes
//               (AccountController.GoogleLogin/FacebookLogin trigger the redirect to the
//               provider; GoogleCallback/FacebookCallback receive the verified identity back
//               and exchange it for the API's own JWT via POST /api/users/external-login).
//               Both providers are registered the same way: only if their AppId/Secret are
//               actually configured (GoogleLoginOptions.cs/FacebookLoginOptions.cs), since
//               registering either scheme with an empty ClientId/AppId crashes the whole site
//               the first time auth middleware runs, not just that provider's button.
// Entities connected: None — this project has no database entities
// Tables related: None — WebApp never touches the database directly, only through the API
// =====================================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using uberworks_webapp.Common;
using uberworks_webapp.Common.Helpers;
using uberworks_webapp.Models.ApiContracts;
using uberworks_webapp.Services.ApiClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Shared by every typed HttpClient below — same base URL and headers for whichever API
// controller they end up calling.
void ConfigureApiClient(HttpClient client)
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException("Api:BaseUrl is not configured in appsettings.");
    var internalSecret = builder.Configuration["Internal:SharedSecret"]
        ?? throw new InvalidOperationException("Internal:SharedSecret is not configured in appsettings/user-secrets.");
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("X-Client-Source", "WebApp");
    client.DefaultRequestHeaders.Add("X-Internal-Secret", internalSecret);
}

builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<IProfessionalsApiClient, ProfessionalsApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<IContactApiClient, ContactApiClient>(ConfigureApiClient);

var authenticationBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

var googleClientId = builder.Configuration["GoogleAuth:ClientId"];
var googleClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];

// Google is only registered once real credentials exist in configuration/user-secrets.
// Google's own OAuthOptions.Validate() throws ArgumentException on an empty ClientId the
// FIRST time any request goes through auth middleware — which would crash the entire site,
// not just the "Continue with Google" button — if we always registered it. This flag also
// drives whether Login.cshtml/Register.cshtml render the Google button at all
// (see IsGoogleLoginEnabled below).
var isGoogleLoginEnabled = !string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret);
builder.Services.AddSingleton(new GoogleLoginOptions(isGoogleLoginEnabled));

if (isGoogleLoginEnabled)
{
    authenticationBuilder.AddGoogle(options =>
    {
        // Google is only used to VERIFY the person's email — the actual session cookie
        // that keeps them logged in on this site is still the one issued above by
        // AddCookie(), built in AccountController.GoogleCallback exactly the same way
        // as a normal Email/Password login.
        options.ClientId = googleClientId!;
        options.ClientSecret = googleClientSecret!;

        // This is where Google's result gets turned into OUR OWN session, not Google's:
        // once Google confirms the person's identity, exchange their verified email for
        // the API's JWT (POST /api/users/external-login) and REPLACE Google's claims with
        // the exact same claim set a normal Email/Password login would produce
        // (AppClaimsFactory.cs). Because SignInScheme wasn't overridden, it defaults to the
        // cookie scheme set above — so by the time AccountController.GoogleCallback runs,
        // the user is already signed in with our claims, not Google's.
        options.Events.OnCreatingTicket = async context =>
        {
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException("Google did not return a verified email address.");
            }

            var firstName = context.Principal?.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
            var lastName = context.Principal?.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

            var usersApiClient = context.HttpContext.RequestServices.GetRequiredService<IUsersApiClient>();
            var auth = await usersApiClient.ExternalLoginAsync(new ExternalLoginRequest
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Provider = AuthProvider.Google
            });

            var identity = AppClaimsFactory.CreateIdentity(auth, context.Scheme.Name);
            context.Principal = new ClaimsPrincipal(identity);
        };

        options.Events.OnRemoteFailure = context =>
        {
            var errorMessage = context.Failure?.Message ?? "Google sign-in failed.";
            context.Response.Redirect("/Account/Login?error=" + Uri.EscapeDataString(errorMessage));
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });
}

var facebookAppId = builder.Configuration["FacebookAuth:AppId"];
var facebookAppSecret = builder.Configuration["FacebookAuth:AppSecret"];

// Same reasoning as isGoogleLoginEnabled above — never register AddFacebook() with an empty
// AppId.
var isFacebookLoginEnabled = !string.IsNullOrWhiteSpace(facebookAppId) && !string.IsNullOrWhiteSpace(facebookAppSecret);
builder.Services.AddSingleton(new FacebookLoginOptions(isFacebookLoginEnabled));

if (isFacebookLoginEnabled)
{
    authenticationBuilder.AddFacebook(options =>
    {
        options.AppId = facebookAppId!;
        options.AppSecret = facebookAppSecret!;
        // Facebook only returns these fields if explicitly requested.
        options.Fields.Add("email");
        options.Fields.Add("first_name");
        options.Fields.Add("last_name");

        // Same exchange as Google's OnCreatingTicket above: verify identity with the
        // provider, then replace its claims with our own (AppClaimsFactory.cs) built from
        // the API's JWT. The only real difference is ProviderUserId — Facebook's numeric
        // user ID gets saved on User.FacebookId (see UserService.ExternalLoginAsync) to link
        // the account, since Facebook accounts are less reliably tied to a stable email than
        // Google accounts are.
        options.Events.OnCreatingTicket = async context =>
        {
            var facebookUserId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException(
                    "Facebook did not return an email address for this account. Please grant email permission, or sign in with a different method.");
            }

            var firstName = context.Principal?.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
            var lastName = context.Principal?.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

            var usersApiClient = context.HttpContext.RequestServices.GetRequiredService<IUsersApiClient>();
            var auth = await usersApiClient.ExternalLoginAsync(new ExternalLoginRequest
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Provider = AuthProvider.Facebook,
                ProviderUserId = facebookUserId
            });

            var identity = AppClaimsFactory.CreateIdentity(auth, context.Scheme.Name);
            context.Principal = new ClaimsPrincipal(identity);
        };

        options.Events.OnRemoteFailure = context =>
        {
            var errorMessage = context.Failure?.Message ?? "Facebook sign-in failed.";
            context.Response.Redirect("/Account/Login?error=" + Uri.EscapeDataString(errorMessage));
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LandingPage}/{id?}")
    .WithStaticAssets();


app.Run();
