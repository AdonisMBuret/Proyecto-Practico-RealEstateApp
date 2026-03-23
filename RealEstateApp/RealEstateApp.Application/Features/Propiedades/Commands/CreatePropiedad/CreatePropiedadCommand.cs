using MediatR;

namespace RealEstateApp.Application.Features.Propiedades.Commands.CreatePropiedad;


public class CreatePropiedadCommand : IRequest<CreatePropiedadResponse>
{
    public string AgenteId { get; set; } = null!;
    public int TipoPropiedadId { get; set; }
    public int TipoVentaId { get; set; }
    public decimal Precio { get; set; }
    public double TamanoEnMetros { get; set; }
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = null!;
    public List<int> MejorasIds { get; set; } = new();
    public List<string> UrlImagenes { get; set; } = new();
}
