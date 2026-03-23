using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.Interfaces.Services;


public interface IFavoritoService
{

    Task AgregarFavoritoAsync(string clienteId, int propiedadId);
    Task RemoverFavoritoAsync(string clienteId, int propiedadId);
    Task<bool> EsFavoritoAsync(string clienteId, int propiedadId);
    
    Task<List<int>> GetPropiedadesFavoritasIdsAsync(string clienteId);
    Task<List<PropiedadViewModel>> GetPropiedadesFavoritasAsync(string clienteId);
    
    Task<int> GetCantidadFavoritosAsync(string clienteId);
}