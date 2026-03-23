using RealEstateApp.Domain.Common;

namespace RealEstateApp.Domain.Entities;

public class Chat : BaseEntity
{
    public DateTime FechaCreacion { get; set; }

    public string ClienteId { get; set; } = null!;
    public string AgenteId { get; set; } = null!;
    public int PropiedadId { get; set; }

    public Propiedad Propiedad { get; set; } = null!;
    
    public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}