using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class Mejora : BaseEntity
{
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

    public ICollection<PropiedadMejora> PropiedadesMejoras { get; set; } = new List<PropiedadMejora>();
}