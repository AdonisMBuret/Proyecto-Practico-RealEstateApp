using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoPropiedades.Queries.GetTipoPropiedadById
{
    public class GetTipoPropiedadByIdQuery : IRequest<TipoPropiedadApiDTO?>
    {
        public int Id { get; set; }
    }
}
