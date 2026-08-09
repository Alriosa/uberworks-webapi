// =====================================================================================
// FILE SUMMARY
// What it does: Defines the type of penalty that can be applied to a user (temporary or
//               permanent). See PenaltyService.CreateAsync for how Permanent penalties get a
//               far-future EndDate sentinel instead of a real null, since
//               TBL_PENALTIES.CL_END_DATE is NOT NULL.
// Entities connected: Penalty.cs
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
