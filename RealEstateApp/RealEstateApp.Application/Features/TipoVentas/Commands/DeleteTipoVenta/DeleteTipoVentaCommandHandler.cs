using MediatR;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.TipoVentas.Commands.DeleteTipoVenta
{
    
    public class DeleteTipoVentaCommandHandler : IRequestHandler<DeleteTipoVentaCommand, Unit>
    {
        private readonly ITipoVentaRepository _repository;

        public DeleteTipoVentaCommandHandler(ITipoVentaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteTipoVentaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }

            return Unit.Value;
        }
    }
}
