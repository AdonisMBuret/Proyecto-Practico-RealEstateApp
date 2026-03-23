using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Mejoras.Commands.UpdateMejora
{
    public class UpdateMejoraCommandHandler : IRequestHandler<UpdateMejoraCommand, MejoraApiDTO>
    {
        private readonly IMejoraRepository _repository;
        private readonly IMapper _mapper;

        public UpdateMejoraCommandHandler(IMejoraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MejoraApiDTO> Handle(UpdateMejoraCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.Id);
            if (existing == null) 
                throw new KeyNotFoundException($"Mejora con ID {request.Id} no encontrada");

            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                existing.Nombre = request.Nombre;
            }

            if (!string.IsNullOrWhiteSpace(request.Descripcion))
            {
                existing.Descripcion = request.Descripcion;
            }

            await _repository.UpdateAsync(existing); 
            return _mapper.Map<MejoraApiDTO>(existing); 
        }
    }
}
