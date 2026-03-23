using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IUserService
{
    Task<List<AgenteViewModel>> GetAllAgentesAsync(bool soloActivos = true);
    Task<AgenteViewModel?> GetAgenteByIdAsync(string id);
    Task<List<AgenteViewModel>> GetAgentesByNombreAsync(string nombre);
    Task<int> GetCantidadPropiedadesByAgenteIdAsync(string agenteId);
}
