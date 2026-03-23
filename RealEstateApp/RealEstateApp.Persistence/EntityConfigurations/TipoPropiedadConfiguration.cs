using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class TipoPropiedadConfiguration : IEntityTypeConfiguration<TipoPropiedad>
{
    public void Configure(EntityTypeBuilder<TipoPropiedad> builder)
    {
        
        builder.ToTable("TiposPropiedades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_TiposPropiedades_Nombre");

        builder.HasMany(x => x.Propiedades)
            .WithOne(x => x.TipoPropiedad)
            .HasForeignKey(x => x.TipoPropiedadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
