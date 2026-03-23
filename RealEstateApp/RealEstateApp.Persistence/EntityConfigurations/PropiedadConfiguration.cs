using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class PropiedadConfiguration : IEntityTypeConfiguration<Propiedad>
{
    public void Configure(EntityTypeBuilder<Propiedad> builder)
    {
        builder.ToTable("Propiedades");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
            .HasMaxLength(10)
            .IsRequired();
            
        builder.Property(x => x.Precio)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(x => x.TamanoEnMetros)
            .HasColumnType("decimal(10,2)")
            .IsRequired();
            
        builder.Property(x => x.CantidadHabitaciones)
            .IsRequired();
            
        builder.Property(x => x.CantidadBanos)
            .IsRequired();
            
        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000)
            .IsRequired();
            
        builder.Property(x => x.Estado)
            .HasConversion<int>()
            .IsRequired();
            
        builder.Property(x => x.AgenteId)
            .HasMaxLength(450)
            .IsRequired();
            
        builder.HasIndex(x => x.Codigo)
            .IsUnique()
            .HasDatabaseName("IX_Propiedades_Codigo");
            
        builder.HasIndex(x => x.Estado);
        builder.HasIndex(x => x.AgenteId);
        builder.HasIndex(x => new { x.Estado, x.TipoPropiedadId });
        builder.HasIndex(x => new { x.Estado, x.TipoVentaId });
        
        builder.HasOne(x => x.TipoPropiedad)
            .WithMany(x => x.Propiedades)
            .HasForeignKey(x => x.TipoPropiedadId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.TipoVenta)
            .WithMany(x => x.Propiedades)
            .HasForeignKey(x => x.TipoVentaId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(x => x.Imagenes)
            .WithOne(x => x.Propiedad)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.PropiedadesMejoras)
            .WithOne(x => x.Propiedad)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.PropiedadesFavoritas)
            .WithOne(x => x.Propiedad)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Ofertas)
            .WithOne(x => x.Propiedad)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(x => x.Chats)
            .WithOne(x => x.Propiedad)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
