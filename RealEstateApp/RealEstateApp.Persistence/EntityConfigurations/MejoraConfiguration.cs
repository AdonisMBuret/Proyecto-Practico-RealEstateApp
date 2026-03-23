using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class MejoraConfiguration : IEntityTypeConfiguration<Mejora>
{
    public void Configure(EntityTypeBuilder<Mejora> builder)
    {
        builder.ToTable("Mejoras");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_Mejoras_Nombre");

        builder.HasMany(x => x.PropiedadesMejoras)
            .WithOne(x => x.Mejora)
            .HasForeignKey(x => x.MejoraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
