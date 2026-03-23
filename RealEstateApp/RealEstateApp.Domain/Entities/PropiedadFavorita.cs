using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class PropiedadFavorita : BaseEntity
{
    public string ClienteId { get; set; } = null!;
    public int PropiedadId { get; set; }

    public Propiedad Propiedad { get; set; } = null!;
}