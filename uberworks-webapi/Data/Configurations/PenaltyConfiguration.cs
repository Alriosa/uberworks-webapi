// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store Penalty.cs in TBL_PENALTIES: real column names.
//               Type, Reason, and EndDate are all required (NOT NULL) — every penalty must
//               state why it was applied, whether it's temporary or permanent, and its end
//               date (set it equal to StartDate for an open-ended/permanent penalty, since
//               the column can't be left empty).
// Entities connected: Penalty.cs (this class configures it; Repository/Service/Controller
//                      not built yet)
// Tables related: TBL_PENALTIES
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class PenaltyConfiguration : IEntityTypeConfiguration<Penalty>
{
    public void Configure(EntityTypeBuilder<Penalty> builder)
    {
        builder.ToTable("TBL_PENALTIES");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("PK_PENALTY_ID")
            .UseIdentityColumn();

        builder.Property(p => p.UserId)
            .HasColumnName("PK_USER_ID");

        builder.Property(p => p.Type)
            .HasColumnName("CL_TYPE")
            .HasConversion(
                type => type.ToString().ToUpperInvariant(),
                value => Enum.Parse<PenaltyType>(value, ignoreCase: true))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Reason)
            .HasColumnName("CL_REASON")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(p => p.StartDate)
            .HasColumnName("CL_START_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.EndDate)
            .HasColumnName("CL_END_DATE")
            .HasColumnType("datetime")
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithMany(u => u.Penalties)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
