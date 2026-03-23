using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.TipoVentas.Queries.GetTipoVentaById
{
    public class GetTipoVentaByIdQueryHandler : IRequestHandler<GetTipoVentaByIdQuery, TipoVentaApiDTO?>
    {
        private readonly ITipoVentaRepository _repository;
        private readonly IMapper _mapper;

        public GetTipoVentaByIdQueryHandler(ITipoVentaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TipoVentaApiDTO?> Handle(GetTipoVentaByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity != null ? _mapper.Map<TipoVentaApiDTO>(entity) : null;
        }
    }
}