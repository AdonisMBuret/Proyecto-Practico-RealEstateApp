using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.UpdateTipoVenta
{
    public class UpdateTipoVentaCommand : IRequest<TipoVentaApiDTO>
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
