using MediatR;
using RealEstateApp.Application.ViewModels.Chat;

namespace RealEstateApp.Application.Features.Chat.Queries.GetChatById;

public class GetChatByIdQuery : IRequest<ChatViewModel>
{
    public int ChatId { get; set; }

    public GetChatByIdQuery(int chatId)
    {
        ChatId = chatId;
    }
}
