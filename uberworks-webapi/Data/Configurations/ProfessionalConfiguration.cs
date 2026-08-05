// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar Professional.cs en TBL_PROFESSIONALS, y define
//           la relación 1:1 con User (un User solo puede tener un Professional, forzado con
//           un índice único sobre la columna PK_USER_ID).
// Entidades relacionadas: Professional.cs (esta clase es la que configura), User.cs (1:1)
// Tablas relacionadas: TBL_PROFESSIONALS
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

        // Relación 1:1 con User (Professional extiende a User).
        builder.HasOne(p => p.User)
            .WithOne(u => u.Professional)
            .HasForeignKey<Professional>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
