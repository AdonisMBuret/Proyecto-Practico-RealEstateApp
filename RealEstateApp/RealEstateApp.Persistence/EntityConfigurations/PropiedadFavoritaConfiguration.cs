using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class PropiedadFavoritaConfiguration : IEntityTypeConfiguration<PropiedadFavorita>
{
    public void Configure(EntityTypeBuilder<PropiedadFavorita> builder)
    {
        // Tabla
        builder.ToTable("PropiedadesFavoritas");

        builder.HasKey(x => x.Id);

        // Propiedades FK
        builder.Property(x => x.ClienteId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.PropiedadId)
            .IsRequired();

        builder.HasIndex(x => new { x.ClienteId, x.PropiedadId })
            .IsUnique()
            .HasDatabaseName("IX_PropiedadesFavoritas_ClienteId_PropiedadId");

        builder.HasIndex(x => x.ClienteId);
        builder.HasIndex(x => x.PropiedadId);

        builder.HasOne(x => x.Propiedad)
            .WithMany(x => x.PropiedadesFavoritas)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
