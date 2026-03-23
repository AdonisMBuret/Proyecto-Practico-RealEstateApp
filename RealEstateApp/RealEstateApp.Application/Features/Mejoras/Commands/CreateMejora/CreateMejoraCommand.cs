using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.Mejoras.Commands.CreateMejora
{
    public class CreateMejoraCommand : IRequest<MejoraApiDTO>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
