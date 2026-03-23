using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Chat.Queries.GetMensajesByChat;

public class GetMensajesByChatQueryHandler : IRequestHandler<GetMensajesByChatQuery, List<MensajeViewModel>>
{
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IMapper _mapper;

    public GetMensajesByChatQueryHandler(
        IMensajeRepository mensajeRepository,
        IMapper mapper)
    {
        _mensajeRepository = mensajeRepository;
        _mapper = mapper;
    }

    public async Task<List<MensajeViewModel>> Handle(GetMensajesByChatQuery request, CancellationToken cancellationToken)
    {
        var mensajes = await _mensajeRepository.GetAllAsync();
        var mensajesChat = mensajes.Where(m => m.ChatId == request.ChatId)
                                    .OrderBy(m => m.FechaEnvio)
                                    .ToList();

        return _mapper.Map<List<MensajeViewModel>>(mensajesChat);
    }
}
