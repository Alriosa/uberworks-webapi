// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store ErrorLog.cs in TBL_ERROR_LOGS. No foreign keys
//               on purpose (see ErrorLog.cs comment) — these rows must never be affected by
//               a user being deleted later.
// Entities connected: ErrorLog.cs (this class configures it)
// Tables related: TBL_ERROR_LOGS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("TBL_ERROR_LOGS");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("PK_ERROR_LOG_ID")
            .UseIdentityColumn();

        builder.Property(e => e.OccurredAt)
            .HasColumnName("CL_OCCURRED_AT")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(e => e.Source)
            .HasColumnName("CL_SOURCE")
            .HasConversion(
                source => source.ToString().ToUpperInvariant(),
                value => Enum.Parse<LogSource>(value, ignoreCase: true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasColumnName("CL_USER_ID");

        builder.Property(e => e.Username)
            .HasColumnName("CL_USERNAME")
            .HasMaxLength(50);

        builder.Property(e => e.RequestMethod)
            .HasColumnName("CL_REQUEST_METHOD")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.RequestPath)
            .HasColumnName("CL_REQUEST_PATH")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.StatusCode)
            .HasColumnName("CL_STATUS_CODE")
            .IsRequired();

        builder.Property(e => e.ExceptionType)
            .HasColumnName("CL_EXCEPTION_TYPE")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Message)
            .HasColumnName("CL_MESSAGE")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(e => e.StackTrace)
            .HasColumnName("CL_STACK_TRACE")
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.IpAddress)
            .HasColumnName("CL_IP_ADDRESS")
            .HasMaxLength(45);

        builder.HasIndex(e => e.OccurredAt);
    }
}
