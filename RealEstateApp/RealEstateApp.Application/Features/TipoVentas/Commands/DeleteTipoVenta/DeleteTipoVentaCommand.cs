using MediatR;

namespace RealEstateApp.Application.Features.TipoVentas.Commands.DeleteTipoVenta
{
    public class DeleteTipoVentaCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteTipoVentaCommand(int id)
        {
            Id = id;
        }
    }
}
