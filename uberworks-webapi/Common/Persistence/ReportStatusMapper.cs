// =====================================================================================
// FILE SUMMARY
// What it does: Converts ReportStatus to/from the text value stored in
//               TBL_REPORTS.CL_STATUS. No special cases — Open/Pending/Resolved/Cancelled
//               uppercase cleanly both ways. Called explicitly by ReportRepository.cs — see
//               UserRoleMapper.cs for why this is a plain static method call instead of a
//               registered Dapper TypeHandler.
// Entities connected: Report.cs
// Tables related: TBL_REPORTS.CL_STATUS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class ReportStatusMapper
{
    public static string ToDb(ReportStatus value) => value.ToString().ToUpperInvariant();
    public static ReportStatus FromDb(string value) => Enum.Parse<ReportStatus>(value, ignoreCase: true);
}
