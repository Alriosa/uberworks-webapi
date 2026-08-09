// =====================================================================================
// FILE SUMMARY
// What it does: Converts PenaltyType to/from the text value stored in
//               TBL_PENALTIES.CL_TYPE. No special cases — Temporary/Permanent uppercase
//               cleanly both ways. Called explicitly by PenaltyRepository.cs — see
//               UserRoleMapper.cs for why this is a plain static method call instead of a
//               registered Dapper TypeHandler.
// Entities connected: Penalty.cs
// Tables related: TBL_PENALTIES.CL_TYPE
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class PenaltyTypeMapper
{
    public static string ToDb(PenaltyType value) => value.ToString().ToUpperInvariant();
    public static PenaltyType FromDb(string value) => Enum.Parse<PenaltyType>(value, ignoreCase: true);
}
