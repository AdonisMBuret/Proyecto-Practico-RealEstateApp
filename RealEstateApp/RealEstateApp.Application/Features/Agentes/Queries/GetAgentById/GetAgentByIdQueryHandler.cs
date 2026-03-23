using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Agentes;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.Interfaces.Services;



namespace RealEstateApp.Application.Features.Agentes.Queries.GetAgentById
{
    public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgenteApiDTO?>
    {
        private readonly IUserService _userService;
        private readonly IPropiedadRepository _propiedadRepository;
        private readonly IMapper _mapper;

        public GetAgentByIdQueryHandler(
            IUserService userService, 
            IPropiedadRepository propiedadRepository,
            IMapper mapper)
        {
            _userService = userService;
            _propiedadRepository = propiedadRepository;
            _mapper = mapper;
        }

        public async Task<AgenteApiDTO?> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
        {
            var agente = await _userService.GetAgenteByIdAsync(request.Id);
            if (agente == null)
                return null;

            var agenteDto = _mapper.Map<AgenteApiDTO>(agente);
            
            agenteDto.CantidadPropiedades = await _propiedadRepository.GetCantidadByAgenteAsync(request.Id);
            
            return agenteDto;
        }
    }
}