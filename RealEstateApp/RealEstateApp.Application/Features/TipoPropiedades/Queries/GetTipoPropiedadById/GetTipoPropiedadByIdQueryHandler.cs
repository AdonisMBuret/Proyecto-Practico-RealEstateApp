using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.TipoPropiedades.Queries.GetTipoPropiedadById
{
    public class GetTipoPropiedadByIdQueryHandler : IRequestHandler<GetTipoPropiedadByIdQuery, TipoPropiedadApiDTO?>
    {
        private readonly ITipoPropiedadRepository _repository;
        private readonly IMapper _mapper;

        public GetTipoPropiedadByIdQueryHandler(ITipoPropiedadRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TipoPropiedadApiDTO?> Handle(GetTipoPropiedadByIdQuery request, CancellationToken cancellationToken)
        {
            var tipoPropiedad = await _repository.GetByIdAsync(request.Id);
            return tipoPropiedad != null ? _mapper.Map<TipoPropiedadApiDTO>(tipoPropiedad) : null;
        }
    }
}