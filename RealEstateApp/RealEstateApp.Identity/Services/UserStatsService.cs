using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Identity.Entities;

namespace RealEstateApp.Identity.Services;

public class UserStatsService : IUserStatsService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserStatsService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(int activos, int inactivos)> GetAgentesStatsAsync()
    {
        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        var activos = agentes.Count(a => a.EmailConfirmed);
        var inactivos = agentes.Count(a => !a.EmailConfirmed);
        return (activos, inactivos);
    }

    public async Task<(int activos, int inactivos)> GetClientesStatsAsync()
    {
        var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
        var activos = clientes.Count(c => c.EmailConfirmed);
        var inactivos = clientes.Count(c => !c.EmailConfirmed);
        return (activos, inactivos);
    }

    public async Task<(int activos, int inactivos)> GetDesarrolladoresStatsAsync()
    {
        var desarrolladores = await _userManager.GetUsersInRoleAsync("Desarrollador");
        var activos = desarrolladores.Count(d => d.EmailConfirmed);
        var inactivos = desarrolladores.Count(d => !d.EmailConfirmed);
        return (activos, inactivos);
    }
}
