using MediatR;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgenteById;


public class GetAgenteByIdQuery : IRequest<AgenteViewModel?>
{
    public string Id { get; set; } = null!;

    public GetAgenteByIdQuery(string id)
    {
        Id = id;
    }
}
