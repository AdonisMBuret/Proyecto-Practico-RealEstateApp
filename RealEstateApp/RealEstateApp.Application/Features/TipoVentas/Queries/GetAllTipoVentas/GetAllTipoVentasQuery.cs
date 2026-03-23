using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoVentas.Queries.GetAllTipoVentas
{
    public class GetAllTipoVentasQuery : IRequest<List<TipoVentaApiDTO>> { }
}
