// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar Review.cs en TBL_REVIEWS, incluyendo el CHECK
//           constraint que obliga a que las calificaciones (1-5) estén en rango válido, y
//           sus tres foreign keys (Professional, Service, User como Client).
// Entidades relacionadas: Review.cs (esta clase es la que configura; aún sin
//                          Repository/Service/Controller construidos)
// Tablas relacionadas: TBL_REVIEWS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("TBL_REVIEWS", t => t.HasCheckConstraint(
            "CK_REVIEWS_RATINGS",
            "(CL_CLIENT_RATING IS NULL OR CL_CLIENT_RATING BETWEEN 1 AND 5) " +
            "AND (CL_PROFESSIONAL_RATING IS NULL OR CL_PROFESSIONAL_RATING BETWEEN 1 AND 5)"));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("PK_REVIEW_ID")
            .UseIdentityColumn();

        builder.Property(r => r.ProfessionalId)
            .HasColumnName("PK_PROFESSIONAL_ID");

        builder.Property(r => r.ServiceId)
            .HasColumnName("PK_SERVICE_ID");

        builder.Property(r => r.ClientId)
            .HasColumnName("CL_CLIENT_ID");

        builder.Property(r => r.ClientRating)
            .HasColumnName("CL_CLIENT_RATING")
            .HasColumnType("tinyint");

        builder.Property(r => r.ProfessionalRating)
            .HasColumnName("CL_PROFESSIONAL_RATING")
            .HasColumnType("tinyint");

        builder.Property(r => r.Comment)
            .HasColumnName("CL_COMMENT")
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.ReviewDate)
            .HasColumnName("CL_REVIEW_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(r => r.Professional)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Service)
            .WithMany(s => s.Reviews)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Client)
            .WithMany(u => u.ReviewsWritten)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
