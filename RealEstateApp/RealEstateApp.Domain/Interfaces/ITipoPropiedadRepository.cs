using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface ITipoPropiedadRepository : IRepositoryAsync<TipoPropiedad>
{
    Task<TipoPropiedad?> GetByIdWithPropiedadesAsync(int id);
    Task<bool> ExisteConNombreAsync(string nombre, int? excludeId = null);
    Task<int> GetCantidadPropiedadesAsync(int tipoPropiedadId);
    Task<List<TipoPropiedad>> GetTiposConPropiedadesAsync();
}
