// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store Professional.cs in TBL_PROFESSIONALS, and
//               defines the 1:1 relationship with User (a User can only have one
//               Professional, enforced with a unique index on the PK_USER_ID column).
//               Description/Experience/Availability/Location are all required (NOT NULL).
//               CompanyUserId is a SEPARATE, optional (nullable) FK to User — a "worker"
//               created by a Company (see ProfessionalService.CreateByCompanyAsync) is
//               still just a Professional, but with this column pointing back at the
//               Company's own User row. DeleteBehavior.Restrict here too, so deleting a
//               Company account can't silently cascade-delete every worker it created.
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
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(p => p.Experience)
            .HasColumnName("CL_EXPERIENCE")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(p => p.Availability)
            .HasColumnName("CL_AVAILABILITY")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Location)
            .HasColumnName("CL_LOCATION")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.AverageRating)
            .HasColumnName("CL_AVERAGE_RATING")
            .HasColumnType("decimal(3,2)")
            .HasDefaultValue(0m);

        builder.Property(p => p.CompanyUserId)
            .HasColumnName("CL_COMPANY_USER_ID");

        // 1:1 relationship with User (Professional extends User).
        builder.HasOne(p => p.User)
            .WithOne(u => u.Professional)
            .HasForeignKey<Professional>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional 1:N relationship: a Company User can own many worker Professionals.
        builder.HasOne(p => p.CompanyUser)
            .WithMany(u => u.ManagedWorkers)
            .HasForeignKey(p => p.CompanyUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
