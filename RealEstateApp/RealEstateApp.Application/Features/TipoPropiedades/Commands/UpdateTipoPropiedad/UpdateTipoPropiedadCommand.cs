using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.UpdateTipoPropiedad
{
    public class UpdateTipoPropiedadCommand : IRequest<TipoPropiedadApiDTO>
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
