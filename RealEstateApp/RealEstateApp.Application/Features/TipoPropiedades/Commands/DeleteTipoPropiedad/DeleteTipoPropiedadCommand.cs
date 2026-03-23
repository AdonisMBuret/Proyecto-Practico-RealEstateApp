using MediatR;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.DeleteTipoPropiedad
{
    
    public class DeleteTipoPropiedadCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteTipoPropiedadCommand(int id)
        {
            Id = id;
        }
    }
}
