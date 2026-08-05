// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store WorkType.cs in TBL_WORKTYPES: real column names
//               and max lengths. It's the simplest table (no foreign keys of its own, only
//               receives one from Service).
// Entities connected: WorkType.cs (this class configures it)
// Tables related: TBL_WORKTYPES
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class WorkTypeConfiguration : IEntityTypeConfiguration<WorkType>
{
    public void Configure(EntityTypeBuilder<WorkType> builder)
    {
        builder.ToTable("TBL_WORKTYPES");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .HasColumnName("PK_WORK_TYPE_ID")
            .UseIdentityColumn();

        builder.Property(w => w.Name)
            .HasColumnName("CL_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasColumnName("CL_DESCRIPTION")
            .HasColumnType("nvarchar(max)");

        builder.Property(w => w.Includes)
            .HasColumnName("CL_INCLUDES")
            .HasColumnType("nvarchar(max)");

        builder.Property(w => w.NotIncludes)
            .HasColumnName("CL_NOT_INCLUDES")
            .HasColumnType("nvarchar(max)");
    }
}
