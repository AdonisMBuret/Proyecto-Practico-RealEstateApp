using MediatR;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgenteById;

public class GetAgenteByIdQueryHandler : IRequestHandler<GetAgenteByIdQuery, AgenteViewModel?>
{
    private readonly IUserService _userService;

    public GetAgenteByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AgenteViewModel?> Handle(GetAgenteByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetAgenteByIdAsync(request.Id);
    }
}
