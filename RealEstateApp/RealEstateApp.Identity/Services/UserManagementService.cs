using Microsoft.AspNetCore.Identity;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Identity.Entities;

namespace RealEstateApp.Identity.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<string[]> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Array.Empty<string>();

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToArray();
    }

    public async Task<bool> ToggleUserStatusAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        user.EmailConfirmed = !user.EmailConfirmed;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<(bool success, string mensaje, string? userId)> CreateUserAsync(
        string nombre, 
        string apellido, 
        string cedula, 
        string email, 
        string nombreUsuario, 
        string password, 
        string telefono, 
        string rol)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = nombreUsuario,
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                Cedula = cedula,
                PhoneNumber = telefono,
                EmailConfirmed = true, 
                EsActivo = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, rol);
                return (true, $"Usuario {user.NombreCompleto} creado exitosamente", user.Id);
            }

            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, errores, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<object?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        return new
        {
            user.Id,
            user.Nombre,
            user.Apellido,
            user.Cedula,
            user.Email,
            user.UserName,
            user.PhoneNumber,
            user.EmailConfirmed,
            user.EsActivo
        };
    }

    public async Task<(bool success, string mensaje)> UpdateUserAsync(
        string userId, 
        string nombre, 
        string apellido, 
        string cedula, 
        string email, 
        string nombreUsuario, 
        string? password, 
        string telefono)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Usuario no encontrado");

            user.Nombre = nombre;
            user.Apellido = apellido;
            user.Cedula = cedula;
            user.Email = email;
            user.UserName = nombreUsuario;
            user.PhoneNumber = telefono;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errores = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errores);
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, password);
                
                if (!passwordResult.Succeeded)
                {
                    var errores = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    return (false, $"Usuario actualizado pero error en contraseña: {errores}");
                }
            }

            return (true, "Usuario actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }
}
