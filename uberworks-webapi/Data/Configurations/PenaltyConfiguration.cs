// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar Penalty.cs en TBL_PENALTIES: nombres de columna
//           reales y su conversión manual de enum nullable (PenaltyType?).
// Entidades relacionadas: Penalty.cs (esta clase es la que configura; aún sin
//                          Repository/Service/Controller construidos)
// Tablas relacionadas: TBL_PENALTIES
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
                type => type == null ? null : type.Value.ToString().ToUpperInvariant(),
                value => value == null ? null : (PenaltyType?)Enum.Parse<PenaltyType>(value, ignoreCase: true))
            .HasMaxLength(50);

        builder.Property(p => p.Reason)
            .HasColumnName("CL_REASON")
            .HasColumnType("nvarchar(max)");

        builder.Property(p => p.StartDate)
            .HasColumnName("CL_START_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.EndDate)
            .HasColumnName("CL_END_DATE")
            .HasColumnType("datetime");

        builder.HasOne(p => p.User)
            .WithMany(u => u.Penalties)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
