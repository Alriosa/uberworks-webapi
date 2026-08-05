// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core, campo por campo, cómo guardar la clase Service.cs en la
//           tabla TBL_SERVICES: qué nombre de columna usar (CL_..., que no coincide con
//           los nombres de C#), qué tipo SQL (nvarchar/decimal/datetime), cuáles son
//           obligatorios, y cómo relacionarse con WorkType y User (foreign keys).
// Entidades relacionadas: Service.cs (esta clase es la que configura)
// Tablas relacionadas: TBL_SERVICES
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("TBL_SERVICES");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("PK_SERVICE_ID")
            .UseIdentityColumn();

        builder.Property(s => s.WorkTypeId)
            .HasColumnName("PK_WORK_TYPE_ID");

        builder.Property(s => s.ClientId)
            .HasColumnName("CL_CLIENT_ID");

        builder.Property(s => s.Description)
            .HasColumnName("CL_DESCRIPTION")
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.ImageUrl)
            .HasColumnName("CL_IMAGE_URL")
            .HasMaxLength(255);

        builder.Property(s => s.ProposedPrice)
            .HasColumnName("CL_PROPOSED_PRICE")
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.Status)
            .HasColumnName("CL_STATUS")
            .HasConversion(
                status => status.ToString().ToUpperInvariant(),
                value => Enum.Parse<ServiceStatus>(value, ignoreCase: true))
            .HasMaxLength(50)
            .HasDefaultValue(ServiceStatus.Pending);

        builder.Property(s => s.RequestDate)
            .HasColumnName("CL_REQUEST_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        // Ubicación: dirección exacta privada (solo dueño / profesional aceptado la ven)
        // vs. zona pública (visible desde el listado abierto).
        builder.Property(s => s.Latitude)
            .HasColumnName("CL_LATITUDE")
            .HasColumnType("decimal(9,6)");

        builder.Property(s => s.Longitude)
            .HasColumnName("CL_LONGITUDE")
            .HasColumnType("decimal(9,6)");

        builder.Property(s => s.ExactAddress)
            .HasColumnName("CL_EXACT_ADDRESS")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.Zone)
            .HasColumnName("CL_ZONE")
            .HasMaxLength(100)
            .IsRequired();

        // Cierre del trabajo: evidencia + confirmación mutua.
        builder.Property(s => s.CompletionPhotoUrl)
            .HasColumnName("CL_COMPLETION_PHOTO_URL")
            .HasMaxLength(255);

        builder.Property(s => s.ClientConfirmedCompletionAt)
            .HasColumnName("CL_CLIENT_CONFIRMED_AT")
            .HasColumnType("datetime");

        builder.Property(s => s.ProfessionalConfirmedCompletionAt)
            .HasColumnName("CL_PROFESSIONAL_CONFIRMED_AT")
            .HasColumnType("datetime");

        builder.HasOne(s => s.WorkType)
            .WithMany(w => w.Services)
            .HasForeignKey(s => s.WorkTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Client)
            .WithMany(u => u.ServicesRequested)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
