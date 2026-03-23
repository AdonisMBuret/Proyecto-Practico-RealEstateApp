using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Ofertas.Queries.GetOfertasByPropiedad;

public class GetOfertasByPropiedadQueryHandler : IRequestHandler<GetOfertasByPropiedadQuery, List<OfertaViewModel>>
{
    private readonly IOfertaRepository _ofertaRepository;
    private readonly IMapper _mapper;

    public GetOfertasByPropiedadQueryHandler(
        IOfertaRepository ofertaRepository,
        IMapper mapper)
    {
        _ofertaRepository = ofertaRepository;
        _mapper = mapper;
    }

    public async Task<List<OfertaViewModel>> Handle(GetOfertasByPropiedadQuery request, CancellationToken cancellationToken)
    {
        var ofertas = await _ofertaRepository.GetAllAsync();
        var ofertasPropiedad = ofertas.Where(o => o.PropiedadId == request.PropiedadId)
                                       .OrderByDescending(o => o.FechaCreacion)
                                       .ToList();

        var viewModels = _mapper.Map<List<OfertaViewModel>>(ofertasPropiedad);
        
        return viewModels;
    }
}
