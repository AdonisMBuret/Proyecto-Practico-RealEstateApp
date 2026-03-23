using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.UpdateTipoVenta
{
    public class UpdateTipoVentaCommandHandler : IRequestHandler<UpdateTipoVentaCommand, TipoVentaApiDTO>
    {
        private readonly ITipoVentaRepository _repository;
        private readonly IMapper _mapper;

        public UpdateTipoVentaCommandHandler(ITipoVentaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TipoVentaApiDTO> Handle(UpdateTipoVentaCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.Id);
            if (existing == null) 
                throw new KeyNotFoundException($"Tipo de venta con ID {request.Id} no encontrado");

            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                existing.Nombre = request.Nombre;
            }

            if (!string.IsNullOrWhiteSpace(request.Descripcion))
            {
                existing.Descripcion = request.Descripcion;
            }

            await _repository.UpdateAsync(existing);
            return _mapper.Map<TipoVentaApiDTO>(existing);
        }
    }
}
