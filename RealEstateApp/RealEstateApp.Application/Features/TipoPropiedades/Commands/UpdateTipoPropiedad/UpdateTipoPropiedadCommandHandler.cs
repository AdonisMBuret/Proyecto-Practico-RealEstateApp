using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.UpdateTipoPropiedad
{
    public class UpdateTipoPropiedadCommandHandler : IRequestHandler<UpdateTipoPropiedadCommand, TipoPropiedadApiDTO>
    {
        private readonly ITipoPropiedadRepository _repository;
        private readonly IMapper _mapper;

        public UpdateTipoPropiedadCommandHandler(ITipoPropiedadRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TipoPropiedadApiDTO> Handle(UpdateTipoPropiedadCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Tipo de propiedad con ID {request.Id} no encontrado");

            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                existing.Nombre = request.Nombre;
            }

            if (!string.IsNullOrWhiteSpace(request.Descripcion))
            {
                existing.Descripcion = request.Descripcion;
            }

            await _repository.UpdateAsync(existing);
            return _mapper.Map<TipoPropiedadApiDTO>(existing); 
        }
    }
}