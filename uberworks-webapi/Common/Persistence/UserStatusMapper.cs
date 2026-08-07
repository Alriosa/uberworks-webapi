// =====================================================================================
// FILE SUMMARY
// What it does: Converts UserStatus to/from the text value stored in TBL_USERS.CL_STATUS.
//               No special cases — ACTIVE/SUSPENDED/PENALIZED uppercase cleanly both ways.
//               Called explicitly by UserRepository.cs — see UserRoleMapper.cs for why this
//               is a plain static method call instead of a registered Dapper TypeHandler.
// Entities connected: User.cs
// Tables related: TBL_USERS.CL_STATUS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class UserStatusMapper
{
    public static string ToDb(UserStatus value) => value.ToString().ToUpperInvariant();
    public static UserStatus FromDb(string value) => Enum.Parse<UserStatus>(value, ignoreCase: true);
}
