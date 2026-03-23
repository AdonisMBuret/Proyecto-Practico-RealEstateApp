using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class ImagenPropiedadConfiguration : IEntityTypeConfiguration<ImagenPropiedad>
{
    public void Configure(EntityTypeBuilder<ImagenPropiedad> builder)
    {
        builder.ToTable("ImagenesPropiedades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UrlImagen)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.EsPrincipal)
            .IsRequired();

        builder.Property(x => x.PropiedadId)
            .IsRequired();

        builder.HasIndex(x => x.PropiedadId);

        builder.HasIndex(x => new { x.PropiedadId, x.EsPrincipal })
            .IsUnique()
            .HasFilter("[EsPrincipal] = 1")
            .HasDatabaseName("IX_ImagenesPropiedades_PropiedadId_Principal");

        builder.HasOne(x => x.Propiedad)
            .WithMany(x => x.Imagenes)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
