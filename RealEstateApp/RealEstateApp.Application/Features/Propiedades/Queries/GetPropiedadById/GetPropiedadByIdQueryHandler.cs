using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadById;


public class GetPropiedadByIdQueryHandler : IRequestHandler<GetPropiedadByIdQuery, PropiedadViewModel?>
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IMapper _mapper;

    public GetPropiedadByIdQueryHandler(IPropiedadRepository propiedadRepository, IMapper mapper)
    {
        _propiedadRepository = propiedadRepository;
        _mapper = mapper;
    }

    public async Task<PropiedadViewModel?> Handle(GetPropiedadByIdQuery request, CancellationToken cancellationToken)
    {
        var propiedad = await _propiedadRepository.GetByIdWithDetailsAsync(request.Id);

        if (propiedad == null)
            return null;

        var propiedadViewModel = _mapper.Map<PropiedadViewModel>(propiedad);

        return propiedadViewModel;
    }
}
