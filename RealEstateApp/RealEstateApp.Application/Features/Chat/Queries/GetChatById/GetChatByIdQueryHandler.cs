using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Chat.Queries.GetChatById;

public class GetChatByIdQueryHandler : IRequestHandler<GetChatByIdQuery, ChatViewModel?>
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;

    public GetChatByIdQueryHandler(
        IChatRepository chatRepository,
        IMapper mapper)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
    }

    public async Task<ChatViewModel?> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        var chat = await _chatRepository.GetByIdAsync(request.ChatId);
        
        if (chat == null)
            return null;

        return _mapper.Map<ChatViewModel>(chat);
    }
}
