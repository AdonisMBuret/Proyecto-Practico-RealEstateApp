using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropertyByCode
{
    public class GetPropertyByCodeQueryHandler : IRequestHandler<GetPropertyByCodeQuery, PropiedadApiDTO?>
    {
        private readonly IPropiedadRepository _propiedadRepository;
        private readonly IMapper _mapper;

        public GetPropertyByCodeQueryHandler(IPropiedadRepository propiedadRepository, IMapper mapper)
        {
            _propiedadRepository = propiedadRepository;
            _mapper = mapper;
        }

        public async Task<PropiedadApiDTO?> Handle(GetPropertyByCodeQuery request, CancellationToken cancellationToken)
        {
            var propiedad = await _propiedadRepository.GetByCodigoAsync(request.Codigo);
            return propiedad != null ? _mapper.Map<PropiedadApiDTO>(propiedad) : null;
        }
    }
}