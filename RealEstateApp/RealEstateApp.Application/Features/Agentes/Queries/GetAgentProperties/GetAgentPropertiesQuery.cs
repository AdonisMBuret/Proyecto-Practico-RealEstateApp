using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgentProperties
{
    public class GetAgentPropertiesQuery : IRequest<List<PropiedadApiDTO>>
    {
        public string AgenteId { get; set; } = string.Empty;
    }
}