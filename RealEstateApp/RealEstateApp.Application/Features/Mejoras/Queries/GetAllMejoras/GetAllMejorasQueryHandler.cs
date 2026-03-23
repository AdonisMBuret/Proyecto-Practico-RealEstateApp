using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.Mejoras.Queries.GetAllMejoras
{
    public class GetAllMejorasQueryHandler : IRequestHandler<GetAllMejorasQuery, List<MejoraApiDTO>>
    {
        private readonly IMejoraRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMejorasQueryHandler(IMejoraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MejoraApiDTO>> Handle(GetAllMejorasQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync();
            return _mapper.Map<List<MejoraApiDTO>>(items);
        }
    }
}
