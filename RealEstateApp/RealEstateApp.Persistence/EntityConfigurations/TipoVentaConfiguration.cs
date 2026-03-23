using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class TipoVentaConfiguration : IEntityTypeConfiguration<TipoVenta>
{
    public void Configure(EntityTypeBuilder<TipoVenta> builder)
    {
        builder.ToTable("TiposVentas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_TiposVentas_Nombre");

        builder.HasMany(x => x.Propiedades)
            .WithOne(x => x.TipoVenta)
            .HasForeignKey(x => x.TipoVentaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
