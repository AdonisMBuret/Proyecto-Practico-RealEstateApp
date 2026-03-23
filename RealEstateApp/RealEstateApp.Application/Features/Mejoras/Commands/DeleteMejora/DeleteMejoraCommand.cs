using MediatR;

namespace RealEstateApp.Application.Features.Mejoras.Commands.DeleteMejora
{
    
    public class DeleteMejoraCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteMejoraCommand(int id)
        {
            Id = id;
        }
    }
}
