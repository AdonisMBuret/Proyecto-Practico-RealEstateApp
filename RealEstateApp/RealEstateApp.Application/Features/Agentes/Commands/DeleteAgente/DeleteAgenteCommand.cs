using MediatR;

namespace RealEstateApp.Application.Features.Agentes.Commands.DeleteAgente;

public class DeleteAgenteCommand : IRequest<DeleteAgenteResponse>
{
    public string AgenteId { get; set; }

    public DeleteAgenteCommand(string agenteId)
    {
        AgenteId = agenteId;
    }
}
