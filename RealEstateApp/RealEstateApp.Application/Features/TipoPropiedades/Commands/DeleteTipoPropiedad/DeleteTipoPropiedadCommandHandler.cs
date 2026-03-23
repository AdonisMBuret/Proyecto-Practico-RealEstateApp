using MediatR;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.TipoPropiedades.Commands.DeleteTipoPropiedad
{
    
    public class DeleteTipoPropiedadCommandHandler : IRequestHandler<DeleteTipoPropiedadCommand, Unit>
    {
        private readonly ITipoPropiedadRepository _repository;

        public DeleteTipoPropiedadCommandHandler(ITipoPropiedadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteTipoPropiedadCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.Id);
            
            if (existing != null)
            {
                await _repository.DeleteAsync(existing);
            }

            return Unit.Value;
        }
    }
}