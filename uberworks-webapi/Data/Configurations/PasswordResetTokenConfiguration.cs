// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store PasswordResetToken.cs in
//               TBL_PASSWORD_RESET_TOKENS. Unlike the audit log tables, this one DOES have
//               a real FK to TBL_USERS (Restrict on delete) — it needs to be queried by
//               UserId when checking whether a user already has an outstanding token.
// Entities connected: PasswordResetToken.cs (this class configures it), User.cs (1:N)
// Tables related: TBL_PASSWORD_RESET_TOKENS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("TBL_PASSWORD_RESET_TOKENS");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("PK_TOKEN_ID")
            .UseIdentityColumn();

        builder.Property(t => t.UserId)
            .HasColumnName("PK_USER_ID");

        builder.Property(t => t.TokenHash)
            .HasColumnName("CL_TOKEN_HASH")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(t => t.TokenHash).IsUnique();

        builder.Property(t => t.ExpiresAt)
            .HasColumnName("CL_EXPIRES_AT")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(t => t.Used)
            .HasColumnName("CL_USED")
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("CL_CREATED_AT")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(t => t.User)
            .WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
