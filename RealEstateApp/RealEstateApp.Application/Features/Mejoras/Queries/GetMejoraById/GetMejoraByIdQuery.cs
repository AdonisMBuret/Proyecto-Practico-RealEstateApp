using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.Mejoras.Queries.GetMejoraById
{
    public class GetMejoraByIdQuery : IRequest<MejoraApiDTO?>
    {
        public int Id { get; set; }
    }
}
