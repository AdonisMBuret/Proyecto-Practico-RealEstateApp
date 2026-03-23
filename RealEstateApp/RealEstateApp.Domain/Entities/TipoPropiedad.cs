using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class TipoPropiedad : BaseEntity
{
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;

    public ICollection<Propiedad> Propiedades { get; set; } = new List<Propiedad>();
}