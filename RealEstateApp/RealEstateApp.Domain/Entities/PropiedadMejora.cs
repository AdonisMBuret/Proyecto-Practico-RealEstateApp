using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class PropiedadMejora : BaseEntity
{
    public int PropiedadId { get; set; }
    public int MejoraId { get; set; }

    public Propiedad Propiedad { get; set; } = null!;
    public Mejora Mejora { get; set; } = null!;
}