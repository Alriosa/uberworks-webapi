// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IAuditLogRepository.cs — just Add + SaveChanges
//               against AppDbContext for each of the 3 log tables. No business logic here;
//               deciding WHAT to log and filling in Source/IpAddress lives in
//               Services/AuditLogService.cs.
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS, TBL_USER_ACTION_LOGS, TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddErrorLogAsync(ErrorLog log)
    {
        _context.ErrorLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task AddUserActionLogAsync(UserActionLog log)
    {
        _context.UserActionLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task AddAdminActionLogAsync(AdminActionLog log)
    {
        _context.AdminActionLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
