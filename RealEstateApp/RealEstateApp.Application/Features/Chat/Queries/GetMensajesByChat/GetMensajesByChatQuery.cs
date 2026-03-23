using MediatR;
using RealEstateApp.Application.ViewModels.Chat;

namespace RealEstateApp.Application.Features.Chat.Queries.GetMensajesByChat;

public class GetMensajesByChatQuery : IRequest<List<MensajeViewModel>>
{
    public int ChatId { get; set; }

    public GetMensajesByChatQuery(int chatId)
    {
        ChatId = chatId;
    }
}
