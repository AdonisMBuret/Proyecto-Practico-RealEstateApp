namespace RealEstateApp.Application.Interfaces.Services;

public interface IGenericService<TViewModel> where TViewModel : class
{
    Task<List<TViewModel>> GetAllAsync();
    Task<TViewModel?> GetByIdAsync(int id);
    Task<TViewModel> CreateAsync(TViewModel viewModel);
    Task UpdateAsync(int id, TViewModel viewModel);
    Task DeleteAsync(int id);
}
