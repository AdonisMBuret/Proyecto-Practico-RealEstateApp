using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IAgenteService
{
    Task<List<AgenteViewModel>> GetAllActivosAsync();
    Task<AgentePerfilViewModel?> GetByIdAsync(string id);
    Task<List<AgenteViewModel>> GetByNombreAsync(string nombre);
    
    Task<AgentePerfilViewModel?> GetPerfilAsync(string agenteId);
    Task<bool> ActualizarPerfilAsync(string agenteId, EditarAgenteViewModel viewModel);
    
    Task<bool> ExisteAgenteAsync(string id);
    Task<bool> EsActivoAsync(string id);
    Task<int> GetCantidadPropiedadesAsync(string agenteId);
}
