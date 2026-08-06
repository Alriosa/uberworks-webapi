// =====================================================================================
// FILE SUMMARY
// What it does: Tells EF Core how to store Chat.cs in TBL_CHATS: real column names and the
//               two foreign keys (to Professional and to User as Client). Message is
//               required (NOT NULL).
// Entities connected: Chat.cs (this class configures it; Repository/Service/Controller
//                      not built yet)
// Tables related: TBL_CHATS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("TBL_CHATS");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("PK_CHAT_ID")
            .UseIdentityColumn();

        builder.Property(c => c.ProfessionalId)
            .HasColumnName("PK_PROFESSIONAL_ID");

        builder.Property(c => c.ClientId)
            .HasColumnName("CL_CLIENT_ID");

        builder.Property(c => c.Message)
            .HasColumnName("CL_MESSAGE")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(c => c.MessageDate)
            .HasColumnName("CL_MESSAGE_DATE")
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(c => c.Professional)
            .WithMany(p => p.Chats)
            .HasForeignKey(c => c.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Client)
            .WithMany(u => u.ChatsAsClient)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
