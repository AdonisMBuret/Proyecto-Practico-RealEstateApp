using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.Application.Interfaces.Services;

public interface ITipoVentaService
{
    Task<List<TipoVentaViewModel>> GetAllAsync();
    Task<TipoVentaViewModel?> GetByIdAsync(int id);
    Task<TipoVentaViewModel> CreateAsync(SaveTipoVentaViewModel viewModel);
    Task<TipoVentaViewModel?> UpdateAsync(int id, SaveTipoVentaViewModel viewModel);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
}
