using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetAllProperties
{
    public class GetAllPropertiesQuery : IRequest<List<PropiedadApiDTO>>
    {
    }
}