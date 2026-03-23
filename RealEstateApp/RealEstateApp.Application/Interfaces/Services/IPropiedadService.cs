using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IPropiedadService
{

    Task<List<PropiedadViewModel>> GetAllDisponiblesAsync();
    Task<PropiedadViewModel?> GetByCodigoAsync(string codigo);
    Task<PropiedadViewModel?> GetByIdAsync(int id);
    Task<List<PropiedadViewModel>> GetByFiltrosAsync(FiltrosPropiedadesViewModel filtros);
    Task<PropiedadDetalleViewModel?> GetDetalleByIdAsync(int id);
    
    Task<List<PropiedadViewModel>> GetByAgenteIdAsync(string agenteId);
    Task<List<PropiedadViewModel>> GetPropiedadesByAgenteAsync(string agenteId, bool incluirVendidas = false);
    
    Task<PropiedadViewModel> CreateAsync(SavePropiedadViewModel viewModel, string agenteId);
    Task<PropiedadViewModel?> UpdateAsync(SavePropiedadViewModel viewModel, string agenteId);
    Task<bool> DeleteAsync(int id, string agenteId);
    
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null);
    Task<bool> EstaDisponibleAsync(int id);
    Task<bool> PerteneceAAgenteAsync(int id, string agenteId);
    Task<PropiedadesEstadisticasViewModel> GetEstadisticasPropiedadesAsync();
    Task<int> GetCantidadPropiedadesByAgenteAsync(string agenteId);
    Task DeleteAllByAgenteAsync(string agenteId);
}
