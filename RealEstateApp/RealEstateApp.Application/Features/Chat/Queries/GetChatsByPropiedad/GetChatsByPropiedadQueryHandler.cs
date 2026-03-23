using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Chat.Queries.GetChatsByPropiedad;

public class GetChatsByPropiedadQueryHandler : IRequestHandler<GetChatsByPropiedadQuery, List<ChatViewModel>>
{
    private readonly IChatRepository _chatRepository;
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IMapper _mapper;

    public GetChatsByPropiedadQueryHandler(
        IChatRepository chatRepository,
        IMensajeRepository mensajeRepository,
        IMapper mapper)
    {
        _chatRepository = chatRepository;
        _mensajeRepository = mensajeRepository;
        _mapper = mapper;
    }

    public async Task<List<ChatViewModel>> Handle(GetChatsByPropiedadQuery request, CancellationToken cancellationToken)
    {
        var chats = await _chatRepository.GetAllAsync();
        var chatsPropiedad = chats.Where(c => c.PropiedadId == request.PropiedadId).ToList();

        var viewModels = new List<ChatViewModel>();

        foreach (var chat in chatsPropiedad)
        {
            var mensajes = await _mensajeRepository.GetAllAsync();
            var ultimoMensaje = mensajes.Where(m => m.ChatId == chat.Id)
                                        .OrderByDescending(m => m.FechaEnvio)
                                        .FirstOrDefault();

            viewModels.Add(new ChatViewModel
            {
                Id = chat.Id,
                PropiedadId = chat.PropiedadId,
                EmisorId = chat.ClienteId?.ToString() ?? string.Empty,
                ReceptorId = chat.AgenteId?.ToString() ?? string.Empty,
                Contenido = ultimoMensaje?.Contenido ?? "Sin mensajes",
                FechaEnvio = ultimoMensaje?.FechaEnvio ?? chat.FechaCreacion,
                EsLeido = false
            });
        }

        return viewModels.OrderByDescending(c => c.FechaEnvio).ToList();
    }
}
