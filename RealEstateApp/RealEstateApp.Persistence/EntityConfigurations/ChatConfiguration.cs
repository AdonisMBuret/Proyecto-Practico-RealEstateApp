using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FechaCreacion)
            .IsRequired();
            
        builder.Property(x => x.ClienteId)
            .HasMaxLength(450)
            .IsRequired();
            
        builder.Property(x => x.AgenteId)
            .HasMaxLength(450)
            .IsRequired();
            
        builder.HasIndex(x => new { x.ClienteId, x.AgenteId, x.PropiedadId })
            .IsUnique()
            .HasDatabaseName("IX_Chats_ClienteId_AgenteId_PropiedadId");
            
        builder.HasIndex(x => x.ClienteId);
        builder.HasIndex(x => x.AgenteId);
        builder.HasIndex(x => x.PropiedadId);
        
        builder.HasOne(x => x.Propiedad)
            .WithMany(x => x.Chats)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Mensajes)
            .WithOne(x => x.Chat)
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}