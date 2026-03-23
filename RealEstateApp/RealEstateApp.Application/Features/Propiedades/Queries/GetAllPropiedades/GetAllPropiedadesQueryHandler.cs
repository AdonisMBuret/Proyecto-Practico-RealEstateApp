using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Propiedades.Queries.GetAllPropiedades;


public class GetAllPropiedadesQueryHandler : IRequestHandler<GetAllPropiedadesQuery, List<PropiedadViewModel>>
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IMapper _mapper;

    public GetAllPropiedadesQueryHandler(IPropiedadRepository propiedadRepository, IMapper mapper)
    {
        _propiedadRepository = propiedadRepository;
        _mapper = mapper;
    }

    public async Task<List<PropiedadViewModel>> Handle(GetAllPropiedadesQuery request, CancellationToken cancellationToken)
    {
        var propiedades = request.SoloDisponibles
            ? await _propiedadRepository.GetAllDisponiblesAsync()  
            : await _propiedadRepository.GetAllAsync();           

        var propiedadesViewModel = _mapper.Map<List<PropiedadViewModel>>(propiedades);

        return propiedadesViewModel;
    }
}
