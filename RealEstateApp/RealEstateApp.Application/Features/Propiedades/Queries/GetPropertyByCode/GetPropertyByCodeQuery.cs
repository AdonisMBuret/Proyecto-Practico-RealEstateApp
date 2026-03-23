using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropertyByCode
{
    public class GetPropertyByCodeQuery : IRequest<PropiedadApiDTO?>
    {
        public string Codigo { get; set; } = string.Empty;
    }
}