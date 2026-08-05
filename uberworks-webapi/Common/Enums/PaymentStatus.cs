// =====================================================================================
// FILE SUMMARY
// What it does: Defines what state a payment is in (pending, held in escrow, released to
//               the professional). Same as PaymentMethod, it exists from the original
//               diagram but doesn't have a Repository/Service/Controller built yet.
// Entities connected: Payment.cs (pending implementation)
// Tables related: TBL_PAYMENTS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_PAYMENTS.CL_STATUS.
/// </summary>
public enum PaymentStatus
{
    Pending,
    Held,
    Released
}
