// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar User.cs en TBL_USERS: nombres de columna reales
//           (CL_FIRST_NAME, CL_EMAIL, etc.), el CHECK constraint de roles válidos
//           (MASTER_ADMIN/ADMIN/CLIENT/PROFESSIONAL), el índice único de Email, y la
//           traducción manual de UserRole (porque "MasterAdmin" en mayúsculas normales
//           daría "MASTERADMIN" en vez de "MASTER_ADMIN").
// Entidades relacionadas: User.cs (esta clase es la que configura)
// Tablas relacionadas: TBL_USERS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("TBL_USERS", t => t.HasCheckConstraint(
            "CK_USERS_ROLE",
            "CL_ROLE IN ('MASTER_ADMIN','ADMIN','CLIENT','PROFESSIONAL')"));

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("PK_USER_ID")
            .UseIdentityColumn();

        builder.Property(u => u.FirstName)
            .HasColumnName("CL_FIRST_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("CL_LAST_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("CL_EMAIL")
            .HasMaxLength(150)
            .IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Phone)
            .HasColumnName("CL_PHONE")
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("CL_PASSWORD")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasColumnName("CL_ROLE")
            .HasConversion(
                role => UserRoleToDb(role),
                value => UserRoleFromDb(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasColumnName("CL_STATUS")
            .HasConversion(
                status => status.ToString().ToUpperInvariant(),
                value => Enum.Parse<UserStatus>(value, ignoreCase: true))
            .HasMaxLength(20)
            .HasDefaultValue(UserStatus.Active);

        builder.Property(u => u.RegistrationDate)
            .HasColumnName("CL_REGISTRATION_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
    }

    // "MasterAdmin" no puede uppercase-arse directo (daría "MASTERADMIN"): el valor
    // guardado en BD usa guion bajo, igual que los estados con nombre compuesto de otras tablas.
    private static string UserRoleToDb(UserRole role) => role switch
    {
        UserRole.MasterAdmin => "MASTER_ADMIN",
        _ => role.ToString().ToUpperInvariant()
    };

    private static UserRole UserRoleFromDb(string value) => value switch
    {
        "MASTER_ADMIN" => UserRole.MasterAdmin,
        _ => Enum.Parse<UserRole>(value, ignoreCase: true)
    };
}
