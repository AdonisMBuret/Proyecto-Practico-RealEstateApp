using MediatR;
using RealEstateApp.Application.DTOs.Agentes;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAllAgents
{
    public class GetAllAgentsQuery : IRequest<List<AgenteApiDTO>>
    {
        public bool SoloActivos { get; set; } = false;
    }
}