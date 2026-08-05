// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store ServiceProfessional.cs in TBL_SERVICE_PROFESSIONALS.
//               Includes a manual translation of the status enum (ServiceProfessionalStatusToDb/
//               FromDb) because the original diagram uses values with spaces like
//               "UNDER NEGOTIATION", which a plain ToUpperInvariant() can't produce on its own.
// Entities connected: ServiceProfessional.cs (this class configures it)
// Tables related: TBL_SERVICE_PROFESSIONALS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class ServiceProfessionalConfiguration : IEntityTypeConfiguration<ServiceProfessional>
{
    public void Configure(EntityTypeBuilder<ServiceProfessional> builder)
    {
        builder.ToTable("TBL_SERVICE_PROFESSIONALS");

        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Id)
            .HasColumnName("PK_SERVICE_PROFESSIONAL_ID")
            .UseIdentityColumn();

        builder.Property(sp => sp.ProfessionalId)
            .HasColumnName("PK_PROFESSIONAL_ID");

        builder.Property(sp => sp.ServiceId)
            .HasColumnName("PK_SERVICE_ID");

        builder.Property(sp => sp.NegotiatedPrice)
            .HasColumnName("CL_NEGOTIATED_PRICE")
            .HasColumnType("decimal(10,2)");

        builder.Property(sp => sp.EstimatedArrivalMinutes)
            .HasColumnName("CL_ESTIMATED_ARRIVAL_MINUTES")
            .IsRequired();

        builder.Property(sp => sp.ArrivalConfirmedAt)
            .HasColumnName("CL_ARRIVAL_CONFIRMED_AT")
            .HasColumnType("datetime");

        builder.Property(sp => sp.Status)
            .HasColumnName("CL_STATUS")
            .HasConversion(
                status => ServiceProfessionalStatusToDb(status),
                value => ServiceProfessionalStatusFromDb(value))
            .HasMaxLength(50)
            .HasDefaultValue(ServiceProfessionalStatus.UnderNegotiation);

        builder.HasOne(sp => sp.Professional)
            .WithMany(p => p.ServiceProfessionals)
            .HasForeignKey(sp => sp.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sp => sp.Service)
            .WithMany(s => s.ServiceProfessionals)
            .HasForeignKey(sp => sp.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // The diagram's values use spaces ("UNDER NEGOTIATION", "IN PROGRESS"), so a generic
    // ToUpperInvariant() isn't enough.
    private static string ServiceProfessionalStatusToDb(ServiceProfessionalStatus status) => status switch
    {
        ServiceProfessionalStatus.UnderNegotiation => "UNDER NEGOTIATION",
        ServiceProfessionalStatus.InProgress => "IN PROGRESS",
        _ => status.ToString().ToUpperInvariant()
    };

    private static ServiceProfessionalStatus ServiceProfessionalStatusFromDb(string value) => value switch
    {
        "UNDER NEGOTIATION" => ServiceProfessionalStatus.UnderNegotiation,
        "IN PROGRESS" => ServiceProfessionalStatus.InProgress,
        _ => Enum.Parse<ServiceProfessionalStatus>(value, ignoreCase: true)
    };
}
