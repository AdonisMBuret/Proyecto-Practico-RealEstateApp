using MediatR;
using RealEstateApp.Application.ViewModels.Ofertas;

namespace RealEstateApp.Application.Features.Ofertas.Queries.GetOfertasByCliente;

public class GetOfertasByClienteQuery : IRequest<List<OfertaViewModel>>
{
    public string ClienteId { get; set; } = null!;
    public int? PropiedadId { get; set; }
}
