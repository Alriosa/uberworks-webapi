// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar WorkType.cs en TBL_WORKTYPES: nombres de
//           columna reales y longitudes máximas. Es la tabla más simple (sin foreign keys
//           propias, solo la reciben desde Service).
// Entidades relacionadas: WorkType.cs (esta clase es la que configura)
// Tablas relacionadas: TBL_WORKTYPES
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
