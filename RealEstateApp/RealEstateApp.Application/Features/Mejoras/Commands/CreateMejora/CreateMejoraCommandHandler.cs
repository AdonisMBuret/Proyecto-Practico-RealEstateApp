using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.Mejoras.Commands.CreateMejora
{
    public class CreateMejoraCommandHandler : IRequestHandler<CreateMejoraCommand, MejoraApiDTO>
    {
        private readonly IMejoraRepository _repository;
        private readonly IMapper _mapper;

        public CreateMejoraCommandHandler(IMejoraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MejoraApiDTO> Handle(CreateMejoraCommand request, CancellationToken cancellationToken)
        {
            var entity = new Mejora { Nombre = request.Nombre, Descripcion = request.Descripcion };
            var created = await _repository.AddAsync(entity);
            return _mapper.Map<MejoraApiDTO>(created);
        }
    }
}
