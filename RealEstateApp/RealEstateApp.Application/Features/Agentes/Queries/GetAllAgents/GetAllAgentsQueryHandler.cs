using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Agentes;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.Interfaces.Services;



namespace RealEstateApp.Application.Features.Agentes.Queries.GetAllAgents
{
    public class GetAllAgentsQueryHandler : IRequestHandler<GetAllAgentsQuery, List<AgenteApiDTO>>
    {
        private readonly IUserService _userService;
        private readonly IPropiedadRepository _propiedadRepository;
        private readonly IMapper _mapper;

        public GetAllAgentsQueryHandler(
            IUserService userService, 
            IPropiedadRepository propiedadRepository,
            IMapper mapper)
        {
            _userService = userService;
            _propiedadRepository = propiedadRepository;
            _mapper = mapper;
        }

        public async Task<List<AgenteApiDTO>> Handle(GetAllAgentsQuery request, CancellationToken cancellationToken)
        {
           
            var agentes = await _userService.GetAllAgentesAsync(request.SoloActivos);
            
            var agentesDTO = new List<AgenteApiDTO>();
            
            foreach (var agente in agentes)
            {
                var agenteDto = _mapper.Map<AgenteApiDTO>(agente);
                
           
                agenteDto.CantidadPropiedades = await _propiedadRepository.GetCantidadByAgenteAsync(agente.Id);
                
                agentesDTO.Add(agenteDto);
            }

            return agentesDTO.OrderBy(a => a.Nombre).ToList();
        }
    }
}