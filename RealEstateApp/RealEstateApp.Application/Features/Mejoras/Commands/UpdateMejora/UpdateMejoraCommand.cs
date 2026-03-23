using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.Mejoras.Commands.UpdateMejora
{
    public class UpdateMejoraCommand : IRequest<MejoraApiDTO>
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
