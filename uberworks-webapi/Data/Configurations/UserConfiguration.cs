// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store User.cs in TBL_USERS: the real column names
//               (CL_FIRST_NAME, CL_EMAIL, etc.), the CHECK constraint of valid roles
//               (MASTER_ADMIN/ADMIN/CLIENT/PROFESSIONAL/MANAGER/COMPANY), the unique index
//               on Email, and the manual translation of UserRole (because "MasterAdmin"
//               uppercased normally would give "MASTERADMIN" instead of "MASTER_ADMIN").
// Entities connected: User.cs (this class configures it)
// Tables related: TBL_USERS
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
            "CL_ROLE IN ('MASTER_ADMIN','ADMIN','CLIENT','PROFESSIONAL','MANAGER','COMPANY')"));

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasColumnName("PK_USER_ID")
            .UseIdentityColumn();

        builder.Property(u => u.Username)
            .HasColumnName("CL_USERNAME")
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();

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

    // "MasterAdmin" can't just be uppercased directly (it would give "MASTERADMIN"): the
    // value stored in the DB uses an underscore, same as the compound-name statuses in
    // other tables.
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
