using MediatR;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Propiedades.Commands.DeletePropiedad;


public class DeletePropiedadCommandHandler : IRequestHandler<DeletePropiedadCommand, DeletePropiedadResponse>
{
    private readonly IPropiedadRepository _propiedadRepository;

    public DeletePropiedadCommandHandler(IPropiedadRepository propiedadRepository)
    {
        _propiedadRepository = propiedadRepository;
    }

    public async Task<DeletePropiedadResponse> Handle(DeletePropiedadCommand request, CancellationToken cancellationToken)
    {
        var propiedad = await _propiedadRepository.GetByIdAsync(request.Id);

        if (propiedad == null)
            throw new KeyNotFoundException($"No se encontró la propiedad con ID {request.Id}");

        await _propiedadRepository.DeleteAsync(propiedad);

        return new DeletePropiedadResponse
        {
            Success = true,
            Mensaje = "Propiedad eliminada exitosamente"
        };
    }
}
