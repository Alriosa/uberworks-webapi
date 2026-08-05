// =====================================================================================
// FILE SUMMARY
// What it does: Runs ONCE every time the API starts (called from Program.cs). Checks
//               whether a user with Role=MasterAdmin already exists in the database; if
//               none exists, it creates one using the email/password configured in
//               appsettings.json (or better, in "dotnet user-secrets") under the
//               "MasterAdmin" section. This way the highest-privilege account never goes
//               through the public registration endpoint (which explicitly rejects it, see
//               Services/UserService.cs → RegisterAsync).
// Entities connected: User.cs (creates a row with Role = UserRole.MasterAdmin)
// Tables related: TBL_USERS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Helpers;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Seed;

/// <summary>
/// Seeds the single MasterAdmin account on API startup, if none exists yet.
/// Never created via /api/users/register — credentials come from configuration
/// (appsettings / user secrets / environment variables), never from source code.
/// </summary>
public static class MasterAdminSeeder
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration, ILogger logger)
    {
        var alreadyExists = await context.Users.AnyAsync(u => u.Role == UserRole.MasterAdmin);
        if (alreadyExists)
        {
            return;
        }

        var email = configuration["MasterAdmin:Email"];
        var password = configuration["MasterAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "No MasterAdmin account exists and MasterAdmin:Email / MasterAdmin:Password " +
                "were not configured. The master account was not seeded.");
            return;
        }

        var masterAdmin = new User
        {
            FirstName = "Master",
            LastName = "Admin",
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            Role = UserRole.MasterAdmin,
            Status = UserStatus.Active
        };

        context.Users.Add(masterAdmin);
        await context.SaveChangesAsync();

        logger.LogInformation("MasterAdmin account seeded for {Email}.", email);
    }
}
