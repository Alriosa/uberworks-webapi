// =====================================================================================
// FILE SUMMARY
// What it does: Converts UserRole to/from the text value stored in TBL_USERS.CL_ROLE (and
//               TBL_ADMIN_ACTION_LOGS.CL_ACTOR_ROLE, which stores the same values) — the
//               same job EF Core's ".HasConversion(...)" used to do. Called EXPLICITLY by
//               each Repository (never registered with Dapper as a "TypeHandler") because
//               Dapper has its own built-in fast-path for enum columns that silently
//               overrides custom TypeHandlers — it would read the string with a plain
//               Enum.Parse (which fails on "MASTER_ADMIN") on the way in, and write the
//               enum's underlying INTEGER instead of its name on the way out. Calling these
//               two methods by hand, visibly, in the SQL-mapping code is what actually works
//               — and it's also easier to read than debugging Dapper's internals.
// Entities connected: User.cs, AdminActionLog.cs
// Tables related: TBL_USERS.CL_ROLE, TBL_ADMIN_ACTION_LOGS.CL_ACTOR_ROLE
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class UserRoleMapper
{
    public static string ToDb(UserRole value) => value switch
    {
        UserRole.MasterAdmin => "MASTER_ADMIN",
        _ => value.ToString().ToUpperInvariant()
    };

    public static UserRole FromDb(string value) => value switch
    {
        "MASTER_ADMIN" => UserRole.MasterAdmin,
        _ => Enum.Parse<UserRole>(value, ignoreCase: true)
    };
}
