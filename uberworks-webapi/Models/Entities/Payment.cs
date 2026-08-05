// =====================================================================================
// FILE SUMMARY
// What it does: Stores the charge associated with a Service (payment method, amount,
//               escrow/release status). Doesn't have a Repository/Service/Controller built
//               yet — one of the pending pieces of a Service's lifecycle.
// Entities connected: Service.cs (N:1)
// Tables related: TBL_PAYMENTS (mapping in Data/Configurations/PaymentConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_PAYMENTS.
/// </summary>
public class Payment
{
    public int Id { get; set; }
    public int ServiceId { get; set; }

    public PaymentMethod? Method { get; set; }
    public decimal? Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; }

    // Navigation properties
    public Service Service { get; set; } = null!;
}
