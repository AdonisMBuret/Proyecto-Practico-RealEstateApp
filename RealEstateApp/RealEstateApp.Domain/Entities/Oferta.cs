using RealEstateApp.Domain.Common;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Domain.Entities;

public class Oferta : BaseEntity
{
    public decimal Monto { get; set; }
    public EstadoOferta Estado { get; set; } = EstadoOferta.Pendiente;
    public DateTime FechaCreacion { get; set; }
    public string? Comentarios { get; set; } 

    public string ClienteId { get; set; } = null!;
    public int PropiedadId { get; set; }

    public Propiedad Propiedad { get; set; } = null!;
}