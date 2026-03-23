using RealEstateApp.Application.ViewModels.Account;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IAccountService
{
    Task<(bool Success, string Message, string? UserId)> LoginAsync(LoginViewModel model);
    Task<(bool Success, string Message)> RegisterAsync(RegistroViewModel model);
    Task<(bool Success, string Message)> ConfirmarCuentaAsync(string userId, string token);
    Task LogoutAsync();
    Task<string?> GetCurrentUserIdAsync();
    Task<string?> GetCurrentUserRoleAsync();
    Task<bool> IsUserInRoleAsync(string userId, string role);
}
