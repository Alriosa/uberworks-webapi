// =====================================================================================
// FILE SUMMARY
// What it does: This is the entry point of the whole API — the first thing that runs when
//               you run "dotnet run". It builds the "pipeline" (the pipe every HTTP request
//               flows through, in order): (1) registers services (DB, Repos, Services, JWT),
//               (2) seeds the MasterAdmin if it doesn't exist, (3) wires up the error
//               middleware, OpenAPI, HTTPS, authentication (reads and validates the JWT),
//               authorization (checks [Authorize]), and rate limiting (caps requests per
//               caller, see Extensions/ServiceCollectionExtensions.AddRateLimiting), and
//               finally (4) wires up the Controllers. The order of app.Use...() calls DOES
//               matter: UseAuthentication() must go BEFORE UseAuthorization() and
//               UseRateLimiter(), because the rate limiter needs to know the caller's
//               identity (from the JWT) to key the per-user limit correctly.
// Entities connected: None directly — orchestrates the startup of the whole app
// Tables related: None directly (though it does trigger the MasterAdmin seed into TBL_USERS)
// =====================================================================================
using uberworks_webapi.Data;
using uberworks_webapi.Data.Seed;
using uberworks_webapi.Extensions;
using uberworks_webapi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimiting();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await MasterAdminSeeder.SeedAsync(dbContext, app.Configuration, logger);
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();
