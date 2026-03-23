using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.TipoVentas.Queries.GetAllTipoVentas
{
    public class GetAllTipoVentasQueryHandler : IRequestHandler<GetAllTipoVentasQuery, List<TipoVentaApiDTO>>
    {
        private readonly ITipoVentaRepository _repository;
        private readonly IMapper _mapper;

        public GetAllTipoVentasQueryHandler(ITipoVentaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TipoVentaApiDTO>> Handle(GetAllTipoVentasQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync();
            return _mapper.Map<List<TipoVentaApiDTO>>(items);
        }
    }
}
