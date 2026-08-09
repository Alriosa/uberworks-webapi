// =====================================================================================
// FILE SUMMARY
// What it does: Converts ReportPaymentOutcome? to/from the text value stored in
//               TBL_REPORTS.CL_PAYMENT_OUTCOME — nullable both ways, since most reports
//               never reach a payment decision (still open, or closed via "Fallo a favor de
//               nadie"/"Cancelar reporte" instead of "Resolver"). Called explicitly by
//               ReportRepository.cs — see UserRoleMapper.cs for why this is a plain static
//               method call instead of a registered Dapper TypeHandler.
// Entities connected: Report.cs
// Tables related: TBL_REPORTS.CL_PAYMENT_OUTCOME
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class ReportPaymentOutcomeMapper
{
    public static string? ToDb(ReportPaymentOutcome? value) => value?.ToString().ToUpperInvariant();
    public static ReportPaymentOutcome? FromDb(string? value) => value is null ? null : Enum.Parse<ReportPaymentOutcome>(value, ignoreCase: true);
}
