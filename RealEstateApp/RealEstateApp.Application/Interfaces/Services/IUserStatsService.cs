namespace RealEstateApp.Application.Interfaces.Services;

public interface IUserStatsService
{
    Task<(int activos, int inactivos)> GetAgentesStatsAsync();
    Task<(int activos, int inactivos)> GetClientesStatsAsync();
    Task<(int activos, int inactivos)> GetDesarrolladoresStatsAsync();
}

public interface IUserManagementService
{
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> ToggleUserStatusAsync(string userId);
    Task<string[]> GetUserRolesAsync(string userId);
    Task<(bool success, string mensaje, string? userId)> CreateUserAsync(string nombre, string apellido, string cedula, string email, string nombreUsuario, string password, string telefono, string rol);
    Task<(bool success, string mensaje)> UpdateUserAsync(string userId, string nombre, string apellido, string cedula, string email, string nombreUsuario, string? password, string telefono);
    Task<object?> GetUserByIdAsync(string userId);
}
