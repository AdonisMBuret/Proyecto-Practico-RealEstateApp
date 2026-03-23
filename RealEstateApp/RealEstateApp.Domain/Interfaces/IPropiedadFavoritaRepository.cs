using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface IPropiedadFavoritaRepository : IRepositoryAsync<PropiedadFavorita>
{
    Task<List<PropiedadFavorita>> GetByClienteIdAsync(string clienteId);
    Task<PropiedadFavorita?> GetByPropiedadAndClienteAsync(int propiedadId, string clienteId);
    Task<bool> IsFavoritaAsync(int propiedadId, string clienteId);
}
