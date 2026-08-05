// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Le dice a EF Core cómo guardar Chat.cs en TBL_CHATS: nombres de columna reales
//           y las dos foreign keys (hacia Professional y hacia User como Client).
// Entidades relacionadas: Chat.cs (esta clase es la que configura; aún sin
//                          Repository/Service/Controller construidos)
// Tablas relacionadas: TBL_CHATS
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
            .HasColumnType("nvarchar(max)");

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
