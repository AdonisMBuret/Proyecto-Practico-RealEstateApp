using MediatR;
using RealEstateApp.Application.ViewModels.Ofertas;

namespace RealEstateApp.Application.Features.Ofertas.Queries.GetOfertasByPropiedad;

public class GetOfertasByPropiedadQuery : IRequest<List<OfertaViewModel>>
{
    public int PropiedadId { get; set; }

    public GetOfertasByPropiedadQuery(int propiedadId)
    {
        PropiedadId = propiedadId;
    }
}
