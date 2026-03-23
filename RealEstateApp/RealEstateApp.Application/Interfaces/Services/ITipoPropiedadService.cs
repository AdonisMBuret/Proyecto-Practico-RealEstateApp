using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.Application.Interfaces.Services;

public interface ITipoPropiedadService
{
    Task<List<TipoPropiedadViewModel>> GetAllAsync();
    Task<TipoPropiedadViewModel?> GetByIdAsync(int id);
    Task<TipoPropiedadViewModel> CreateAsync(SaveTipoPropiedadViewModel viewModel);
    Task UpdateAsync(int id, SaveTipoPropiedadViewModel viewModel);
    Task DeleteAsync(int id);
    Task<bool> ExisteAsync(int id);
}
