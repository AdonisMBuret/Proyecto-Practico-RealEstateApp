using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.TipoPropiedades.Queries.GetAllTipoPropiedades
{
    public class GetAllTipoPropiedadesQueryHandler : IRequestHandler<GetAllTipoPropiedadesQuery, List<TipoPropiedadApiDTO>>
    {
        private readonly ITipoPropiedadRepository _repository;
        private readonly IMapper _mapper;

        public GetAllTipoPropiedadesQueryHandler(ITipoPropiedadRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TipoPropiedadApiDTO>> Handle(GetAllTipoPropiedadesQuery request, CancellationToken cancellationToken)
        {
            var tiposPropiedades = await _repository.GetAllAsync();
            return _mapper.Map<List<TipoPropiedadApiDTO>>(tiposPropiedades);
        }
    }
}