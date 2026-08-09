// =====================================================================================
// FILE SUMMARY
// What it does: Records what happened to a Service's held payment (see PaymentStatus.cs —
//               Held) when a Report was closed via the Support dashboard's "Resolver"
//               button: the Support agent must choose whether the held money goes to the
//               professional or back to the client before the report can close that way.
//               Null on a Report means either it's still open, or it was closed via "Fallo a
//               favor de nadie" (no special payment decision — the hold is simply lifted and
//               the normal payment flow proceeds) or "Cancelar reporte" (no payment decision
//               at all). See ReportService.ResolveAsync/NoFaultAsync.
// Entities connected: Report.cs (the Report.PaymentOutcome property is of this type)
// Tables related: TBL_REPORTS.CL_PAYMENT_OUTCOME
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

public enum ReportPaymentOutcome
{
    ReleasedToProfessional,
    RefundedToClient
}
