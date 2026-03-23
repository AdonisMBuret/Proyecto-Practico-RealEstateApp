using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropertyById
{
    public class GetPropertyByIdQuery : IRequest<PropiedadApiDTO?>
    {
        public int Id { get; set; }
    }
}