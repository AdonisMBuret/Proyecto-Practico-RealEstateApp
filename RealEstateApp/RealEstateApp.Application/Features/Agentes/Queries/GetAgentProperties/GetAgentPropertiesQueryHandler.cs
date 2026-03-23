using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.Interfaces.Services;



namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgentProperties
{
    public class GetAgentPropertiesQueryHandler : IRequestHandler<GetAgentPropertiesQuery, List<PropiedadApiDTO>>
    {
        private readonly IUserService _userService;
        private readonly IPropiedadRepository _propiedadRepository;
        private readonly IMapper _mapper;

        public GetAgentPropertiesQueryHandler(
            IUserService userService,
            IPropiedadRepository propiedadRepository,
            IMapper mapper)
        {
            _userService = userService;
            _propiedadRepository = propiedadRepository;
            _mapper = mapper;
        }

        public async Task<List<PropiedadApiDTO>> Handle(GetAgentPropertiesQuery request, CancellationToken cancellationToken)
        {
       
            var agente = await _userService.GetAgenteByIdAsync(request.AgenteId);
            if (agente == null)
                return new List<PropiedadApiDTO>();

           
            var propiedades = await _propiedadRepository.GetByAgenteIdAsync(request.AgenteId);
            
            return _mapper.Map<List<PropiedadApiDTO>>(propiedades);
        }
    }
}