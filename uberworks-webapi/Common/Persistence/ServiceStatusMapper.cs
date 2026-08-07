// =====================================================================================
// FILE SUMMARY
// What it does: Converts ServiceStatus to/from the text value stored in
//               TBL_SERVICES.CL_STATUS. No special cases — Pending/Accepted/Cancelled/
//               Completed uppercase cleanly both ways. Called explicitly by
//               ServiceRepository.cs — see UserRoleMapper.cs for why this is a plain static
//               method call instead of a registered Dapper TypeHandler.
// Entities connected: Service.cs
// Tables related: TBL_SERVICES.CL_STATUS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class ServiceStatusMapper
{
    public static string ToDb(ServiceStatus value) => value.ToString().ToUpperInvariant();
    public static ServiceStatus FromDb(string value) => Enum.Parse<ServiceStatus>(value, ignoreCase: true);
}
