using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;


public interface IMejoraRepository : IRepositoryAsync<Mejora>
{
    Task<List<Mejora>> GetAllActiveAsync();
    Task<List<Mejora>> GetByIdsAsync(List<int> ids);
    Task<int> GetCantidadPropiedadesAsync(int mejoraId);
    Task<bool> ExistsWithNameAsync(string nombre, int? excludeId = null);
}
