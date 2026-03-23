using MediatR;
using RealEstateApp.Application.ViewModels.Chat;

namespace RealEstateApp.Application.Features.Chat.Queries.GetChatsByPropiedad;

public class GetChatsByPropiedadQuery : IRequest<List<ChatViewModel>>
{
    public int PropiedadId { get; set; }

    public GetChatsByPropiedadQuery(int propiedadId)
    {
        PropiedadId = propiedadId;
    }
}
