using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.CreateTipoVenta
{
    public class CreateTipoVentaCommand : IRequest<TipoVentaApiDTO>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
