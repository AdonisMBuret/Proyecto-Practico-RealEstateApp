using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface ITipoVentaRepository : IRepositoryAsync<TipoVenta>
{
    Task<TipoVenta?> GetByIdWithPropiedadesAsync(int id);
    Task<int> GetCantidadPropiedadesAsync(int tipoVentaId);
    Task<bool> ExistsWithNameAsync(string nombre, int? excludeId = null);
}
