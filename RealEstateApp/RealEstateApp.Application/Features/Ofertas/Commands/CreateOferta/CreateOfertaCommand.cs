using MediatR;

namespace RealEstateApp.Application.Features.Ofertas.Commands.CreateOferta;

public class CreateOfertaCommand : IRequest<CreateOfertaResponse>
{
    public int PropiedadId { get; set; }
    public string ClienteId { get; set; } = null!;
    public decimal Monto { get; set; }
}
