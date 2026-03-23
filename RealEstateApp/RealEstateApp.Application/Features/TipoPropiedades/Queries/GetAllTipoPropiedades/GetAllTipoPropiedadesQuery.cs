using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoPropiedades.Queries.GetAllTipoPropiedades
{
    public class GetAllTipoPropiedadesQuery : IRequest<List<TipoPropiedadApiDTO>>
    {
    }
}
