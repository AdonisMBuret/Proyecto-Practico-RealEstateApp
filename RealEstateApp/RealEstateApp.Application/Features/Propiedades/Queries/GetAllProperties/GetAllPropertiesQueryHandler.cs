using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.Propiedades.Queries.GetAllProperties
{
    public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, List<PropiedadApiDTO>>
    {
        private readonly IPropiedadRepository _propiedadRepository;
        private readonly IMapper _mapper;

        public GetAllPropertiesQueryHandler(IPropiedadRepository propiedadRepository, IMapper mapper)
        {
            _propiedadRepository = propiedadRepository;
            _mapper = mapper;
        }

        public async Task<List<PropiedadApiDTO>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            var propiedades = await _propiedadRepository.GetAllAsync();
            return _mapper.Map<List<PropiedadApiDTO>>(propiedades);
        }
    }
}