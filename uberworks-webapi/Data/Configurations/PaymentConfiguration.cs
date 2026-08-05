// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store Payment.cs in TBL_PAYMENTS, including the CHECK
//               constraint on the payment method (CREDITCARD/PAYPAL/ZELLE) and its manual
//               nullable enum conversion (PaymentMethod?) to text and back.
// Entities connected: Payment.cs (this class configures it; Repository/Service/Controller
//                      not built yet)
// Tables related: TBL_PAYMENTS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("TBL_PAYMENTS", t => t.HasCheckConstraint(
            "CK_PAYMENTS_METHOD",
            "CL_METHOD IS NULL OR CL_METHOD IN ('CREDITCARD','PAYPAL','ZELLE')"));

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("PK_PAYMENT_ID")
            .UseIdentityColumn();

        builder.Property(p => p.ServiceId)
            .HasColumnName("PK_SERVICE_ID");

        builder.Property(p => p.Method)
            .HasColumnName("CL_METHOD")
            .HasConversion(
                method => method == null ? null : PaymentMethodToDb(method.Value),
                value => value == null ? null : (PaymentMethod?)PaymentMethodFromDb(value))
            .HasMaxLength(50);

        builder.Property(p => p.Amount)
            .HasColumnName("CL_AMOUNT")
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Status)
            .HasColumnName("CL_STATUS")
            .HasConversion(
                status => status.ToString().ToUpperInvariant(),
                value => Enum.Parse<PaymentStatus>(value, ignoreCase: true))
            .HasMaxLength(50)
            .HasDefaultValue(PaymentStatus.Pending);

        builder.Property(p => p.PaymentDate)
            .HasColumnName("CL_PAYMENT_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(p => p.Service)
            .WithMany(s => s.Payments)
            .HasForeignKey(p => p.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static string PaymentMethodToDb(PaymentMethod method) => method switch
    {
        PaymentMethod.CreditCard => "CREDITCARD",
        PaymentMethod.PayPal => "PAYPAL",
        PaymentMethod.Zelle => "ZELLE",
        _ => method.ToString().ToUpperInvariant()
    };

    private static PaymentMethod PaymentMethodFromDb(string value) => value switch
    {
        "CREDITCARD" => PaymentMethod.CreditCard,
        "PAYPAL" => PaymentMethod.PayPal,
        "ZELLE" => PaymentMethod.Zelle,
        _ => Enum.Parse<PaymentMethod>(value, ignoreCase: true)
    };
}
