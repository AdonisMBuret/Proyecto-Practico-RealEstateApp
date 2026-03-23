using MediatR;

namespace RealEstateApp.Application.Features.Ofertas.Commands.RechazarOferta;

public class RechazarOfertaCommand : IRequest<RechazarOfertaResponse>
{
    public int OfertaId { get; set; }
    public string AgenteId { get; set; } = null!;
}
