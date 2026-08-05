// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store Reward.cs in TBL_REWARDS, and enforces (via a
//               unique index) that each User has at most one points record, treating it as
//               a balance that gets updated, not an event history.
// Entities connected: Reward.cs (this class configures it), User.cs (1:1)
// Tables related: TBL_REWARDS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class RewardConfiguration : IEntityTypeConfiguration<Reward>
{
    public void Configure(EntityTypeBuilder<Reward> builder)
    {
        builder.ToTable("TBL_REWARDS");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("PK_REWARD_ID")
            .UseIdentityColumn();

        builder.Property(r => r.UserId)
            .HasColumnName("PK_USER_ID");

        // NOTE: the diagram doesn't mark this FK as UNIQUE (unlike TBL_PROFESSIONALS), but
        // it's modeled here as 1 points record per user (a balance that gets updated, not a
        // history). Let me know if you actually want to allow multiple rows per user.
        builder.HasIndex(r => r.UserId).IsUnique();

        builder.Property(r => r.Points)
            .HasColumnName("CL_POINTS")
            .HasDefaultValue(0);

        builder.Property(r => r.LastUpdateDate)
            .HasColumnName("CL_LAST_UPDATE_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(r => r.User)
            .WithOne(u => u.Reward)
            .HasForeignKey<Reward>(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
