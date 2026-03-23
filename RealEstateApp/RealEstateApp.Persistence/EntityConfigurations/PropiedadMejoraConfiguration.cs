using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Persistence.EntityConfigurations;


public class PropiedadMejoraConfiguration : IEntityTypeConfiguration<PropiedadMejora>
{
    public void Configure(EntityTypeBuilder<PropiedadMejora> builder)
    {
        
        builder.ToTable("PropiedadesMejoras");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PropiedadId)
            .IsRequired();

        builder.Property(x => x.MejoraId)
            .IsRequired();

        builder.HasIndex(x => new { x.PropiedadId, x.MejoraId })
            .IsUnique()
            .HasDatabaseName("IX_PropiedadesMejoras_PropiedadId_MejoraId");

        builder.HasIndex(x => x.PropiedadId);
        builder.HasIndex(x => x.MejoraId);

        builder.HasOne(x => x.Propiedad)
            .WithMany(x => x.PropiedadesMejoras)
            .HasForeignKey(x => x.PropiedadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Mejora)
            .WithMany(x => x.PropiedadesMejoras)
            .HasForeignKey(x => x.MejoraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
