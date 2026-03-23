using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface IPropiedadRepository : IRepositoryAsync<Propiedad>
{
    Task<List<Propiedad>> GetAllDisponiblesAsync();
    Task<List<Propiedad>> GetAllWithDetailsAsync();
    Task<Propiedad?> GetByIdWithDetailsAsync(int id);
    Task<Propiedad?> GetByCodigoAsync(string codigo);
    Task<List<Propiedad>> GetByFiltrosAsync(int? tipoPropiedadId = null, decimal? precioMin = null, decimal? precioMax = null, int? habitaciones = null, int? banos = null);
    Task<Propiedad?> GetDetalleByIdAsync(int id);
    
    Task<List<Propiedad>> GetByAgenteIdAsync(string agenteId, bool soloDisponibles = true);
    Task<int> GetCantidadByAgenteAsync(string agenteId);
    
    Task<bool> EstaDisponibleAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null);
    
    Task<string> GenerarCodigoAsync();
    Task<(int Disponibles, int Vendidas)> GetEstadisticasAsync();
    
    Task DeleteAllByAgenteAsync(string agenteId);
    
    // Métodos para manejar mejoras
    Task AddPropiedadMejoraAsync(PropiedadMejora propiedadMejora);
    Task RemovePropiedadMejorasAsync(int propiedadId);
}
