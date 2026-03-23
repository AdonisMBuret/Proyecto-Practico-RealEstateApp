using MediatR;

namespace RealEstateApp.Application.Features.Chat.Commands.EnviarMensaje;

public class EnviarMensajeCommand : IRequest<EnviarMensajeResponse>
{
    public int PropiedadId { get; set; }
    public string EmisorId { get; set; } = string.Empty;
    public string ReceptorId { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
}
