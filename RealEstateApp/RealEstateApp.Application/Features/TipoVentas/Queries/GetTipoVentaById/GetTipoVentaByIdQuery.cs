using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoVentas.Queries.GetTipoVentaById
{
    public class GetTipoVentaByIdQuery : IRequest<TipoVentaApiDTO?>
    {
        public int Id { get; set; }
    }
}