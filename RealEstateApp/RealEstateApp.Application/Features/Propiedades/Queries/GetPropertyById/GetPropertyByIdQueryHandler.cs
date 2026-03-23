using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.Propiedades.Queries.GetPropertyById
{
    public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropiedadApiDTO?>
    {
        private readonly IPropiedadRepository _propiedadRepository;
        private readonly IMapper _mapper;

        public GetPropertyByIdQueryHandler(IPropiedadRepository propiedadRepository, IMapper mapper)
        {
            _propiedadRepository = propiedadRepository;
            _mapper = mapper;
        }

        public async Task<PropiedadApiDTO?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var propiedad = await _propiedadRepository.GetByIdAsync(request.Id);
            return propiedad != null ? _mapper.Map<PropiedadApiDTO>(propiedad) : null;
        }
    }
}