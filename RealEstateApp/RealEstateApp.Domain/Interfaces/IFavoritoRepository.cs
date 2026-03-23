using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface IFavoritoRepository : IRepositoryAsync<PropiedadFavorita>
{
    Task<bool> EsFavoritoAsync(string clienteId, int propiedadId);
    Task<PropiedadFavorita?> GetByClienteYPropiedadAsync(string clienteId, int propiedadId);
    Task<List<int>> GetPropiedadesFavoritasIdsAsync(string clienteId);
    Task<List<Propiedad>> GetPropiedadesFavoritasAsync(string clienteId);
    Task<int> GetCantidadFavoritosAsync(string clienteId);
}