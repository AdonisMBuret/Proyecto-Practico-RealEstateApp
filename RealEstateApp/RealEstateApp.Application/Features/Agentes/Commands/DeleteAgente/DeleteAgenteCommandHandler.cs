using MediatR;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Features.Agentes.Commands.DeleteAgente;

public class DeleteAgenteCommandHandler : IRequestHandler<DeleteAgenteCommand, DeleteAgenteResponse>
{
    private readonly IRepositoryAsync<Propiedad> _propiedadesRepository;
    private readonly IUserManagementService _userManagementService;

    public DeleteAgenteCommandHandler(
        IRepositoryAsync<Propiedad> propiedadesRepository,
        IUserManagementService userManagementService)
    {
        _propiedadesRepository = propiedadesRepository;
        _userManagementService = userManagementService;
    }

    public async Task<DeleteAgenteResponse> Handle(DeleteAgenteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _userManagementService.GetUserRolesAsync(request.AgenteId);
            if (!roles.Contains("Agente"))
            {
                return new DeleteAgenteResponse
                {
                    Success = false,
                    Mensaje = "El usuario no es un agente"
                };
            }

            var propiedades = await _propiedadesRepository.GetAllAsync();
            var propiedadesAgente = propiedades.Where(p => p.AgenteId == request.AgenteId).ToList();

            foreach (var propiedad in propiedadesAgente)
            {
                await _propiedadesRepository.DeleteAsync(propiedad);
            }

            var deleted = await _userManagementService.DeleteUserAsync(request.AgenteId);

            if (deleted)
            {
                return new DeleteAgenteResponse
                {
                    Success = true,
                    Mensaje = $"Agente y sus {propiedadesAgente.Count} propiedades eliminados exitosamente"
                };
            }

            return new DeleteAgenteResponse
            {
                Success = false,
                Mensaje = "Error al eliminar el agente"
            };
        }
        catch (Exception ex)
        {
            return new DeleteAgenteResponse
            {
                Success = false,
                Mensaje = $"Error: {ex.Message}"
            };
        }
    }
}
