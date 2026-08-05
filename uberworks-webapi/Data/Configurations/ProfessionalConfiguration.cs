// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store Professional.cs in TBL_PROFESSIONALS, and
//               defines the 1:1 relationship with User (a User can only have one
//               Professional, enforced with a unique index on the PK_USER_ID column).
// Entities connected: Professional.cs (this class configures it), User.cs (1:1)
// Tables related: TBL_PROFESSIONALS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.ToTable("TBL_PROFESSIONALS");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasColumnName("PK_PROFESSIONAL_ID")
            .UseIdentityColumn();

        builder.Property(p => p.UserId)
            .HasColumnName("PK_USER_ID");
        builder.HasIndex(p => p.UserId).IsUnique();

        builder.Property(p => p.Description)
            .HasColumnName("CL_DESCRIPTION")
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.Experience)
            .HasColumnName("CL_EXPERIENCE")
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.Availability)
            .HasColumnName("CL_AVAILABILITY")
            .HasMaxLength(100);

        builder.Property(p => p.Location)
            .HasColumnName("CL_LOCATION")
            .HasMaxLength(200);

        builder.Property(p => p.AverageRating)
            .HasColumnName("CL_AVERAGE_RATING")
            .HasColumnType("decimal(3,2)")
            .HasDefaultValue(0m);

        // 1:1 relationship with User (Professional extends User).
        builder.HasOne(p => p.User)
            .WithOne(u => u.Professional)
            .HasForeignKey<Professional>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
