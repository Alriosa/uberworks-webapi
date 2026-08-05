// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IAuditLogService.cs. Pulls Source/IpAddress
//               automatically from ICurrentUserService (so every call site doesn't have to
//               pass them manually), builds the entity, and writes it via
//               IAuditLogRepository. Every method is wrapped in try/catch: if writing the
//               log itself fails for any reason (DB hiccup, etc.), it's reported to the
//               normal application logger (ILogger, goes to console/file) but NEVER
//               re-thrown — a broken audit log must never take down the actual feature a
//               user is using, and it especially must never cause a second unhandled
//               exception when called FROM the error handler itself (LogErrorAsync).
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS, TBL_USER_ACTION_LOGS, TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        IAuditLogRepository auditLogRepository,
        ICurrentUserService currentUserService,
        ILogger<AuditLogService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task LogUserActionAsync(
        int? actorUserId,
        string? actorUsername,
        string action,
        string? targetEntityType = null,
        int? targetEntityId = null,
        string? details = null)
    {
        try
        {
            await _auditLogRepository.AddUserActionLogAsync(new UserActionLog
            {
                OccurredAt = DateTime.UtcNow,
                Source = _currentUserService.Source,
                ActorUserId = actorUserId,
                ActorUsername = actorUsername,
                Action = action,
                TargetEntityType = targetEntityType,
                TargetEntityId = targetEntityId,
                Details = details,
                IpAddress = _currentUserService.IpAddress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write UserActionLog for action {Action}.", action);
        }
    }

    public async Task LogAdminActionAsync(
        int actorUserId,
        string actorUsername,
        UserRole actorRole,
        string action,
        string? targetEntityType = null,
        int? targetEntityId = null,
        string? details = null)
    {
        try
        {
            await _auditLogRepository.AddAdminActionLogAsync(new AdminActionLog
            {
                OccurredAt = DateTime.UtcNow,
                Source = _currentUserService.Source,
                ActorUserId = actorUserId,
                ActorUsername = actorUsername,
                ActorRole = actorRole,
                Action = action,
                TargetEntityType = targetEntityType,
                TargetEntityId = targetEntityId,
                Details = details,
                IpAddress = _currentUserService.IpAddress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write AdminActionLog for action {Action}.", action);
        }
    }

    public async Task LogErrorAsync(Exception exception, string requestMethod, string requestPath, int statusCode)
    {
        try
        {
            await _auditLogRepository.AddErrorLogAsync(new ErrorLog
            {
                OccurredAt = DateTime.UtcNow,
                Source = _currentUserService.Source,
                UserId = _currentUserService.UserId,
                Username = _currentUserService.Username,
                RequestMethod = requestMethod,
                RequestPath = requestPath,
                StatusCode = statusCode,
                ExceptionType = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                IpAddress = _currentUserService.IpAddress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write ErrorLog for exception {ExceptionType}.", exception.GetType().Name);
        }
    }
}
