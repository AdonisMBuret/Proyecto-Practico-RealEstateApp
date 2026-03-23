using MediatR;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgentesByNombre;

public class GetAgentesByNombreQueryHandler : IRequestHandler<GetAgentesByNombreQuery, List<AgenteViewModel>>
{
    private readonly IUserService _userService;

    public GetAgentesByNombreQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<List<AgenteViewModel>> Handle(GetAgentesByNombreQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetAgentesByNombreAsync(request.Nombre);
    }
}
