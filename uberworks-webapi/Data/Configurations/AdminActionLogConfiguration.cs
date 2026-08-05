// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store AdminActionLog.cs in TBL_ADMIN_ACTION_LOGS. No
//               foreign keys on purpose (see AdminActionLog.cs comment).
// Entities connected: AdminActionLog.cs (this class configures it)
// Tables related: TBL_ADMIN_ACTION_LOGS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class AdminActionLogConfiguration : IEntityTypeConfiguration<AdminActionLog>
{
    public void Configure(EntityTypeBuilder<AdminActionLog> builder)
    {
        builder.ToTable("TBL_ADMIN_ACTION_LOGS");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("PK_ADMIN_ACTION_LOG_ID")
            .UseIdentityColumn();

        builder.Property(a => a.OccurredAt)
            .HasColumnName("CL_OCCURRED_AT")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(a => a.Source)
            .HasColumnName("CL_SOURCE")
            .HasConversion(
                source => source.ToString().ToUpperInvariant(),
                value => Enum.Parse<LogSource>(value, ignoreCase: true))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.ActorUserId)
            .HasColumnName("CL_ACTOR_USER_ID")
            .IsRequired();

        builder.Property(a => a.ActorUsername)
            .HasColumnName("CL_ACTOR_USERNAME")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.ActorRole)
            .HasColumnName("CL_ACTOR_ROLE")
            .HasConversion(
                role => AdminActionActorRoleToDb(role),
                value => AdminActionActorRoleFromDb(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Action)
            .HasColumnName("CL_ACTION")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.TargetEntityType)
            .HasColumnName("CL_TARGET_ENTITY_TYPE")
            .HasMaxLength(100);

        builder.Property(a => a.TargetEntityId)
            .HasColumnName("CL_TARGET_ENTITY_ID");

        builder.Property(a => a.Details)
            .HasColumnName("CL_DETAILS")
            .HasColumnType("nvarchar(max)");

        builder.Property(a => a.IpAddress)
            .HasColumnName("CL_IP_ADDRESS")
            .HasMaxLength(45);

        builder.HasIndex(a => a.OccurredAt);
        builder.HasIndex(a => a.ActorUserId);
    }

    // Reuses the same MASTER_ADMIN spelling as UserConfiguration.cs for consistency.
    private static string AdminActionActorRoleToDb(UserRole role) => role switch
    {
        UserRole.MasterAdmin => "MASTER_ADMIN",
        _ => role.ToString().ToUpperInvariant()
    };

    private static UserRole AdminActionActorRoleFromDb(string value) => value switch
    {
        "MASTER_ADMIN" => UserRole.MasterAdmin,
        _ => Enum.Parse<UserRole>(value, ignoreCase: true)
    };
}
