using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.CreateTipoPropiedad
{
    public class CreateTipoPropiedadCommandHandler : IRequestHandler<CreateTipoPropiedadCommand, TipoPropiedadApiDTO>
    {
        private readonly ITipoPropiedadRepository _repository;
        private readonly IMapper _mapper;

        public CreateTipoPropiedadCommandHandler(ITipoPropiedadRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TipoPropiedadApiDTO> Handle(CreateTipoPropiedadCommand request, CancellationToken cancellationToken)
        {
            var tipoPropiedad = new TipoPropiedad
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion
            };

            var created = await _repository.AddAsync(tipoPropiedad);
            return _mapper.Map<TipoPropiedadApiDTO>(created);
        }
    }
}