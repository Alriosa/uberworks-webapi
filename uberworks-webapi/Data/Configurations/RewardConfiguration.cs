// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar Reward.cs en TBL_REWARDS, y fuerza (vía índice
//           único) que cada User tenga como máximo un registro de puntos, tratándolo como
//           un saldo que se actualiza, no un historial de eventos.
// Entidades relacionadas: Reward.cs (esta clase es la que configura), User.cs (1:1)
// Tablas relacionadas: TBL_REWARDS
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

        // NOTA: el diagrama no marca esta FK como UNIQUE (a diferencia de TBL_PROFESSIONALS),
        // pero se modela como 1 registro de puntos por usuario (saldo que se actualiza,
        // no un historial). Avísame si en realidad quieres permitir múltiples filas por usuario.
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
