using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class MensajeConfiguration : IEntityTypeConfiguration<Mensaje>
{
    public void Configure(EntityTypeBuilder<Mensaje> builder)
    {
        builder.ToTable("Mensajes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Contenido)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.FechaEnvio)
            .IsRequired();

        builder.Property(x => x.EsLeido)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.EmisorId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.ReceptorId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.ChatId)
            .IsRequired();

        builder.HasIndex(x => x.ChatId);
        builder.HasIndex(x => x.EmisorId);
        builder.HasIndex(x => x.ReceptorId);
        builder.HasIndex(x => x.FechaEnvio);
        builder.HasIndex(x => x.EsLeido);

        builder.HasIndex(x => new { x.ChatId, x.FechaEnvio });
        builder.HasIndex(x => new { x.ReceptorId, x.EsLeido });

        builder.HasOne(x => x.Chat)
            .WithMany(x => x.Mensajes)
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
