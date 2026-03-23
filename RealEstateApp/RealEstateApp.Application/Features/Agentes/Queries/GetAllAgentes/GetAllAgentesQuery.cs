using MediatR;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAllAgentes;


public class GetAllAgentesQuery : IRequest<List<AgenteViewModel>>
{
    public bool SoloActivos { get; set; } = true;
}
