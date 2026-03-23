using AutoMapper;
using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.Mejoras.Queries.GetMejoraById
{
    public class GetMejoraByIdQueryHandler : IRequestHandler<GetMejoraByIdQuery, MejoraApiDTO?>
    {
        private readonly IMejoraRepository _repository;
        private readonly IMapper _mapper;

        public GetMejoraByIdQueryHandler(IMejoraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MejoraApiDTO?> Handle(GetMejoraByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity != null ? _mapper.Map<MejoraApiDTO>(entity) : null;
        }
    }
}
