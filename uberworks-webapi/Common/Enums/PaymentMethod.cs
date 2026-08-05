// =====================================================================================
// FILE SUMMARY
// What it does: Defines what method was used to pay for a Service (card, PayPal, Zelle).
//               There is no real charging logic wired up yet — this enum exists because it
//               was already in the original database diagram (TBL_PAYMENTS), ready for
//               whenever that entity gets built out.
// Entities connected: Payment.cs (Repository/Service/Controller not implemented yet)
// Tables related: TBL_PAYMENTS.CL_METHOD
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the CHECK constraint on TBL_PAYMENTS.CL_METHOD.
/// </summary>
public enum PaymentMethod
{
    CreditCard,
    PayPal,
    Zelle
}
