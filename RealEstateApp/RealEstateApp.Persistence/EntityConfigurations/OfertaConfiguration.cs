using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class OfertaConfiguration : IEntityTypeConfiguration<Oferta>
{
    public void Configure(EntityTypeBuilder<Oferta> builder)
    {
        // Tabla
        builder.ToTable("Ofertas");

        // Clave primaria
        builder.HasKey(x => x.Id);

        // Propiedades FK
        builder.Property(x => x.ClienteId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.PropiedadId)
            .IsRequired();

        // Propiedades
        builder.Property(x => x.Monto)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Estado)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        // Índices
        builder.HasIndex(x => x.ClienteId);
        builder.HasIndex(x => x.PropiedadId);
        builder.HasIndex(x => x.Estado);
        builder.HasIndex(x => x.FechaCreacion);

        // Índice compuesto para búsquedas comunes
        builder.HasIndex(x => new { x.PropiedadId, x.Estado });

        // Relación con Propiedad
        builder.HasOne(x => x.Propiedad)
            .WithMany(x => x.Ofertas)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
