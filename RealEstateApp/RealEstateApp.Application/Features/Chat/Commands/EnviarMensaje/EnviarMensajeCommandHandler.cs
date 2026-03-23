using MediatR;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Chat.Commands.EnviarMensaje;

public class EnviarMensajeCommandHandler : IRequestHandler<EnviarMensajeCommand, EnviarMensajeResponse>
{
    private readonly IChatRepository _chatRepository;
    private readonly IMensajeRepository _mensajeRepository;

    public EnviarMensajeCommandHandler(
        IChatRepository chatRepository,
        IMensajeRepository mensajeRepository)
    {
        _chatRepository = chatRepository;
        _mensajeRepository = mensajeRepository;
    }

    public async Task<EnviarMensajeResponse> Handle(EnviarMensajeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar o crear chat
            var chats = await _chatRepository.GetAllAsync();
            var chat = chats.FirstOrDefault(c => 
                c.PropiedadId == request.PropiedadId &&
                ((c.ClienteId == request.EmisorId && c.AgenteId == request.ReceptorId) ||
                 (c.ClienteId == request.ReceptorId && c.AgenteId == request.EmisorId)));

            if (chat == null)
            {
                
                // Determinar quién es cliente y quién es agente
                chat = new Domain.Entities.Chat
                {
                    PropiedadId = request.PropiedadId,
                    ClienteId = request.EmisorId, 
                    AgenteId = request.ReceptorId,  
                    FechaCreacion = DateTime.Now
                };
                await _chatRepository.AddAsync(chat);
            }

            // Crear mensaje
            var mensaje = new Mensaje
            {
                ChatId = chat.Id,
                EmisorId = request.EmisorId,
                ReceptorId = request.ReceptorId,
                Contenido = request.Contenido,
                FechaEnvio = DateTime.Now,
                EsLeido = false
            };

            await _mensajeRepository.AddAsync(mensaje);

            return new EnviarMensajeResponse
            {
                Success = true,
                Mensaje = "Mensaje enviado exitosamente",
                MensajeId = mensaje.Id
            };
        }
        catch (Exception ex)
        {
            return new EnviarMensajeResponse
            {
                Success = false,
                Mensaje = $"Error al enviar mensaje: {ex.Message}"
            };
        }
    }
}
