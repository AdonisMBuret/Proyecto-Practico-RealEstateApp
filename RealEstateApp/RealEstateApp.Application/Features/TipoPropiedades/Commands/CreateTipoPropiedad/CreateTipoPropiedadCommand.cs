using MediatR;
using RealEstateApp.Application.DTOs.Mantenimientos;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.CreateTipoPropiedad
{
    
    public class CreateTipoPropiedadCommand : IRequest<TipoPropiedadApiDTO>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
