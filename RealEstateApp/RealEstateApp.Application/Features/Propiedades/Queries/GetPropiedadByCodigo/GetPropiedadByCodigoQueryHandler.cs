using MediatR;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadByCodigo;


public class GetPropiedadByCodigoQueryHandler : IRequestHandler<GetPropiedadByCodigoQuery, PropiedadViewModel?>
{
    private readonly IPropiedadService _propiedadService;

    public GetPropiedadByCodigoQueryHandler(IPropiedadService propiedadService)
    {
        _propiedadService = propiedadService;
    }

    public async Task<PropiedadViewModel?> Handle(GetPropiedadByCodigoQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo))
            return null;

        return await _propiedadService.GetByCodigoAsync(request.Codigo);
    }
}