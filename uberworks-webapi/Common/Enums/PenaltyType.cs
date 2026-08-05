// =====================================================================================
// FILE SUMMARY
// What it does: Defines the type of penalty that can be applied to a user (temporary or
//               permanent). Exists from the original diagram; the Penalty.cs entity doesn't
//               have a Repository/Service/Controller built yet.
// Entities connected: Penalty.cs (pending implementation)
// Tables related: TBL_PENALTIES.CL_TYPE
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_PENALTIES.CL_TYPE.
/// </summary>
public enum PenaltyType
{
    Temporary,
    Permanent
}
