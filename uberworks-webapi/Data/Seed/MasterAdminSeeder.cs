// =====================================================================================
// FILE SUMMARY
// What it does: Runs ONCE every time the API starts (called from Program.cs). Checks
//               whether a user with Role=MasterAdmin already exists in the database; if
//               none exists, it creates one using the email/password configured in
//               appsettings.json (or better, in "dotnet user-secrets") under the
//               "MasterAdmin" section. This way the highest-privilege account never goes
//               through the public registration endpoint (which explicitly rejects it, see
//               Services/UserService.cs → RegisterAsync). The creation itself is recorded
//               directly into TBL_ADMIN_ACTION_LOGS (bypassing IAuditLogService, since there
//               is no HTTP request/IP/logged-in caller at startup to pull from) so there's
//               still a permanent audit trail of exactly when the master account was
//               created. Talks straight to the database via IDbConnectionFactory (Dapper),
//               same as every Repository — startup code doesn't get a shortcut around that.
//               Role/Status/Source/ActorRole are converted to their DB string explicitly
//               via UserRoleMapper.cs/UserStatusMapper.cs/LogSourceMapper.cs before being
//               passed to Dapper — see UserRoleMapper.cs's FILE SUMMARY for why passing the
//               enum value directly would silently insert the wrong thing.
// Entities connected: User.cs (creates a row with Role = UserRole.MasterAdmin),
//                      AdminActionLog.cs (records the creation event)
// Tables related: TBL_USERS, TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Helpers;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;

namespace uberworks_webapi.Data.Seed;

/// <summary>
/// Seeds the single MasterAdmin account on API startup, if none exists yet.
/// Never created via /api/users/register — credentials come from configuration
/// (appsettings / user secrets / environment variables), never from source code.
/// </summary>
public static class MasterAdminSeeder
{
    public static async Task SeedAsync(IDbConnectionFactory connectionFactory, IConfiguration configuration, ILogger logger)
    {
        using var connection = connectionFactory.CreateConnection();

        var alreadyExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM TBL_USERS WHERE CL_ROLE = 'MASTER_ADMIN') THEN 1 ELSE 0 END");
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

        const string insertUserSql = """
            INSERT INTO TBL_USERS (CL_USERNAME, CL_FIRST_NAME, CL_LAST_NAME, CL_EMAIL, CL_PASSWORD, CL_ROLE, CL_STATUS)
            OUTPUT INSERTED.PK_USER_ID
            VALUES (@Username, @FirstName, @LastName, @Email, @PasswordHash, @Role, @Status)
            """;

        var masterAdminId = await connection.ExecuteScalarAsync<int>(insertUserSql, new
        {
            Username = "masteradmin",
            FirstName = "Master",
            LastName = "Admin",
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            Role = UserRoleMapper.ToDb(UserRole.MasterAdmin),
            Status = UserStatusMapper.ToDb(UserStatus.Active)
        });

        const string insertLogSql = """
            INSERT INTO TBL_ADMIN_ACTION_LOGS
                (CL_OCCURRED_AT, CL_SOURCE, CL_ACTOR_USER_ID, CL_ACTOR_USERNAME, CL_ACTOR_ROLE, CL_ACTION,
                 CL_TARGET_ENTITY_TYPE, CL_TARGET_ENTITY_ID, CL_DETAILS)
            VALUES
                (@OccurredAt, @Source, @ActorUserId, @ActorUsername, @ActorRole, @Action,
                 @TargetEntityType, @TargetEntityId, @Details)
            """;

        await connection.ExecuteAsync(insertLogSql, new
        {
            OccurredAt = DateTime.UtcNow,
            Source = LogSourceMapper.ToDb(LogSource.Direct),
            ActorUserId = masterAdminId,
            ActorUsername = "masteradmin",
            ActorRole = UserRoleMapper.ToDb(UserRole.MasterAdmin),
            Action = "MASTER_ADMIN_SEEDED",
            TargetEntityType = "User",
            TargetEntityId = masterAdminId,
            Details = $"MasterAdmin account created automatically on API startup for {email}."
        });

        logger.LogInformation("MasterAdmin account seeded for {Email}.", email);
    }
}
