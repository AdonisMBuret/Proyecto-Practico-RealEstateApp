using MediatR;

namespace RealEstateApp.Application.Features.Propiedades.Commands.DeletePropiedad;


public class DeletePropiedadCommand : IRequest<DeletePropiedadResponse>
{
    public int Id { get; set; }

    public DeletePropiedadCommand(int id)
    {
        Id = id;
    }
}
