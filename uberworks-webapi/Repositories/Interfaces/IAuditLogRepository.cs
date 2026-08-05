// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for writing to the 3 log tables. Deliberately
//               write-only (no Get methods) — reading/querying the logs for an audit is
//               meant to happen directly against the database (SSMS, a BI tool, etc.) or
//               via a future dedicated reporting endpoint, not through this app's normal
//               request flow.
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS, TBL_USER_ACTION_LOGS, TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task AddErrorLogAsync(ErrorLog log);
    Task AddUserActionLogAsync(UserActionLog log);
    Task AddAdminActionLogAsync(AdminActionLog log);
}
