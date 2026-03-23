using MediatR;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Mejoras.Commands.DeleteMejora
{
   
    public class DeleteMejoraCommandHandler : IRequestHandler<DeleteMejoraCommand, Unit>
    {
        private readonly IMejoraRepository _repository;

        public DeleteMejoraCommandHandler(IMejoraRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteMejoraCommand request, CancellationToken cancellationToken)
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
