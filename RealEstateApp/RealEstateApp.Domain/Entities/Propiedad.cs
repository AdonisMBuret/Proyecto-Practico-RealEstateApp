using RealEstateApp.Domain.Common;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Domain.Entities;

public class Propiedad : BaseEntity
{
    public string Codigo { get; set; } = null!;
    public decimal Precio { get; set; }
    public double TamanoEnMetros { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = null!;
    public EstadoPropiedad Estado { get; set; } = EstadoPropiedad.Disponible;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public int TipoPropiedadId { get; set; }
    public int TipoVentaId { get; set; }
    public string AgenteId { get; set; } = null!;

    public TipoPropiedad TipoPropiedad { get; set; } = null!;
    public TipoVenta TipoVenta { get; set; } = null!;
    
    public ICollection<ImagenPropiedad> Imagenes { get; set; } = new List<ImagenPropiedad>();
    public ICollection<PropiedadMejora> PropiedadesMejoras { get; set; } = new List<PropiedadMejora>();
    public ICollection<PropiedadFavorita> PropiedadesFavoritas { get; set; } = new List<PropiedadFavorita>();
    public ICollection<Oferta> Ofertas { get; set; } = new List<Oferta>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}