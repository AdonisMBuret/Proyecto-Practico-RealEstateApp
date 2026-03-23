using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.Mejoras.Queries.GetAllMejoras
{
    public class GetAllMejorasQuery : IRequest<List<MejoraApiDTO>> { }
}
