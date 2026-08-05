// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es el punto de entrada de toda la API — lo primero que se ejecuta al correr
//           "dotnet run". Arma el "pipeline" (la tubería por la que pasa cada petición
//           HTTP, en orden): (1) registra servicios (DB, Repos, Services, JWT), (2) siembra
//           el MasterAdmin si no existe, (3) conecta el middleware de errores, OpenAPI,
//           HTTPS, autenticación (lee y valida el JWT) y autorización (revisa [Authorize]),
//           y finalmente (4) conecta los Controllers. El orden de app.Use...() SÍ importa:
//           UseAuthentication() debe ir ANTES que UseAuthorization(), porque primero hay
//           que saber quién eres, y después decidir si tienes permiso.
// Entidades relacionadas: Ninguna directamente — orquesta el arranque de toda la app
// Tablas relacionadas: Ninguna directamente (aunque dispara el seed de MasterAdmin en TBL_USERS)
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

app.MapControllers();

app.Run();
