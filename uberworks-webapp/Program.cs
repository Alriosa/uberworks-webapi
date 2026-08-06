// =====================================================================================
// FILE SUMMARY
// What it does: Entry point of the WebApp. Registers MVC (Controllers + Views), a typed
//               HttpClient for talking to uberworks-webapi (base URL from
//               appsettings.json → Api:BaseUrl, with "X-Client-Source: WebApp" attached to
//               every outgoing request so the API's audit logs can tell this traffic apart
//               from Mobile or direct calls — see ICurrentUserService.Source in the API),
//               and cookie-based authentication (so once a user logs in via
//               AccountController, their identity — and the API's JWT — persists across
//               page requests without the browser having to resend credentials each time).
// Entities connected: None — this project has no database entities
// Tables related: None — WebApp never touches the database directly, only through the API
// =====================================================================================
using Microsoft.AspNetCore.Authentication.Cookies;
using uberworks_webapp.Services.ApiClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(client =>
{
    var baseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException("Api:BaseUrl is not configured in appsettings.");
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("X-Client-Source", "WebApp");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

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
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
