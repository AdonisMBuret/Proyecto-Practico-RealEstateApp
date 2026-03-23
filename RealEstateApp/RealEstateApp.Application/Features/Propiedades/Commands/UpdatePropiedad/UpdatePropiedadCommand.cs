using MediatR;

namespace RealEstateApp.Application.Features.Propiedades.Commands.UpdatePropiedad;


public class UpdatePropiedadCommand : IRequest<UpdatePropiedadResponse>
{
    public int Id { get; set; }
    public int TipoPropiedadId { get; set; }
    public int TipoVentaId { get; set; }
    public decimal Precio { get; set; }
    public double TamanoEnMetros { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = null!;
    public List<int> MejorasIds { get; set; } = new();
    public List<string> UrlImagenesNuevas { get; set; } = new();
    public List<string> UrlImagenesExistentes { get; set; } = new();
}
