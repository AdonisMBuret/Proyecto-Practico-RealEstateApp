using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IMejoraService
{
    Task<List<MejoraViewModel>> GetAllAsync();
    Task<MejoraViewModel?> GetByIdAsync(int id);
    Task<MejoraViewModel> CreateAsync(SaveMejoraViewModel viewModel);
    Task<MejoraViewModel?> UpdateAsync(int id, SaveMejoraViewModel viewModel);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
}
