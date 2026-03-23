using MediatR;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAllAgentes;


public class GetAllAgentesQueryHandler : IRequestHandler<GetAllAgentesQuery, List<AgenteViewModel>>
{
    private readonly IUserService _userService;

    public GetAllAgentesQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<List<AgenteViewModel>> Handle(GetAllAgentesQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetAllAgentesAsync(request.SoloActivos);
    }
}
