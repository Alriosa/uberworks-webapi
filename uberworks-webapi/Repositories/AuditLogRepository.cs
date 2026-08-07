// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IAuditLogRepository.cs — one INSERT per log
//               table, plain SQL, no joins (these 3 tables have no foreign keys at all, see
//               each entity's FILE SUMMARY for why). Source (all 3 tables) and ActorRole
//               (AdminActionLog only) are converted to their DB string explicitly via
//               LogSourceMapper.cs/UserRoleMapper.cs before being passed to Dapper — see
//               UserRoleMapper.cs's FILE SUMMARY for why passing the enum value directly
//               would silently insert the wrong thing. No business logic here; deciding
//               WHAT to log and filling in Source/IpAddress lives in
//               Services/AuditLogService.cs.
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS, TBL_USER_ACTION_LOGS, TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using Dapper;
using uberworks_webapi.Common.Persistence;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuditLogRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddErrorLogAsync(ErrorLog log)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_ERROR_LOGS
                (CL_OCCURRED_AT, CL_SOURCE, CL_USER_ID, CL_USERNAME, CL_REQUEST_METHOD,
                 CL_REQUEST_PATH, CL_STATUS_CODE, CL_EXCEPTION_TYPE, CL_MESSAGE, CL_STACK_TRACE, CL_IP_ADDRESS)
            VALUES
                (@OccurredAt, @Source, @UserId, @Username, @RequestMethod,
                 @RequestPath, @StatusCode, @ExceptionType, @Message, @StackTrace, @IpAddress)
            """;

        await connection.ExecuteAsync(sql, new
        {
            log.OccurredAt,
            Source = LogSourceMapper.ToDb(log.Source),
            log.UserId,
            log.Username,
            log.RequestMethod,
            log.RequestPath,
            log.StatusCode,
            log.ExceptionType,
            log.Message,
            log.StackTrace,
            log.IpAddress
        });
    }

    public async Task AddUserActionLogAsync(UserActionLog log)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_USER_ACTION_LOGS
                (CL_OCCURRED_AT, CL_SOURCE, CL_ACTOR_USER_ID, CL_ACTOR_USERNAME, CL_ACTION,
                 CL_TARGET_ENTITY_TYPE, CL_TARGET_ENTITY_ID, CL_DETAILS, CL_IP_ADDRESS)
            VALUES
                (@OccurredAt, @Source, @ActorUserId, @ActorUsername, @Action,
                 @TargetEntityType, @TargetEntityId, @Details, @IpAddress)
            """;

        await connection.ExecuteAsync(sql, new
        {
            log.OccurredAt,
            Source = LogSourceMapper.ToDb(log.Source),
            log.ActorUserId,
            log.ActorUsername,
            log.Action,
            log.TargetEntityType,
            log.TargetEntityId,
            log.Details,
            log.IpAddress
        });
    }

    public async Task AddAdminActionLogAsync(AdminActionLog log)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO TBL_ADMIN_ACTION_LOGS
                (CL_OCCURRED_AT, CL_SOURCE, CL_ACTOR_USER_ID, CL_ACTOR_USERNAME, CL_ACTOR_ROLE, CL_ACTION,
                 CL_TARGET_ENTITY_TYPE, CL_TARGET_ENTITY_ID, CL_DETAILS, CL_IP_ADDRESS)
            VALUES
                (@OccurredAt, @Source, @ActorUserId, @ActorUsername, @ActorRole, @Action,
                 @TargetEntityType, @TargetEntityId, @Details, @IpAddress)
            """;

        await connection.ExecuteAsync(sql, new
        {
            log.OccurredAt,
            Source = LogSourceMapper.ToDb(log.Source),
            log.ActorUserId,
            log.ActorUsername,
            ActorRole = UserRoleMapper.ToDb(log.ActorRole),
            log.Action,
            log.TargetEntityType,
            log.TargetEntityId,
            log.Details,
            log.IpAddress
        });
    }
}
