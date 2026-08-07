// =====================================================================================
// FILE SUMMARY
// What it does: Converts LogSource to/from the text value stored in CL_SOURCE (all 3 log
//               tables). No special cases — Direct/WebApp/MobileApp uppercase cleanly both
//               ways. Called explicitly by AuditLogRepository.cs/MasterAdminSeeder.cs — see
//               UserRoleMapper.cs for why this is a plain static method call instead of a
//               registered Dapper TypeHandler.
// Entities connected: ErrorLog.cs, UserActionLog.cs, AdminActionLog.cs
// Tables related: TBL_ERROR_LOGS.CL_SOURCE, TBL_USER_ACTION_LOGS.CL_SOURCE,
//                 TBL_ADMIN_ACTION_LOGS.CL_SOURCE
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class LogSourceMapper
{
    public static string ToDb(LogSource value) => value.ToString().ToUpperInvariant();
    public static LogSource FromDb(string value) => Enum.Parse<LogSource>(value, ignoreCase: true);
}
