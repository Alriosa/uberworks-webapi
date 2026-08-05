// =====================================================================================
// FILE SUMMARY
// What it does: Defines what stage a "Work Post" (Service) is in: just created (Pending),
//               with a professional already accepted (Accepted), cancelled, or closed
//               (Completed, which is only reached once both client and professional confirm
//               separately).
// Entities connected: Service.cs (the Service.Status property is of this type)
// Tables related: TBL_SERVICES.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_SERVICES.CL_STATUS.
/// </summary>
public enum ServiceStatus
{
    Pending,
    Accepted,
    Cancelled,
    Completed
}
