using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.CreateTipoVenta
{
    public class CreateTipoVentaCommandHandler : IRequestHandler<CreateTipoVentaCommand, TipoVentaApiDTO>
    {
        private readonly ITipoVentaRepository _repository;
        private readonly IMapper _mapper;

        public CreateTipoVentaCommandHandler(ITipoVentaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TipoVentaApiDTO> Handle(CreateTipoVentaCommand request, CancellationToken cancellationToken)
        {
            var entity = new TipoVenta { Nombre = request.Nombre, Descripcion = request.Descripcion };
            var created = await _repository.AddAsync(entity);
            return _mapper.Map<TipoVentaApiDTO>(created);
        }
    }
}
