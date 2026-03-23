using MediatR;
using RealEstateApp.Application.DTOs.Agentes;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgentById
{
    public class GetAgentByIdQuery : IRequest<AgenteApiDTO?>
    {
        public string Id { get; set; } = string.Empty;
    }
}