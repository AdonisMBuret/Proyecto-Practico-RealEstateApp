using MediatR;

namespace RealEstateApp.Application.Features.Ofertas.Commands.AceptarOferta;

public class AceptarOfertaCommand : IRequest<AceptarOfertaResponse>
{
    public int OfertaId { get; set; }
    public string AgenteId { get; set; } = null!;
}
