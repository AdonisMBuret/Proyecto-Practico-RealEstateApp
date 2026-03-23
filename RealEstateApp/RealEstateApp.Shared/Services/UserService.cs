using Microsoft.AspNetCore.Identity;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Identity.Entities;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Shared.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPropiedadRepository _propiedadRepository;
    private const string ROLE_AGENTE = "Agente";

    public UserService(
        UserManager<ApplicationUser> userManager,
        IPropiedadRepository propiedadRepository)
    {
        _userManager = userManager;
        _propiedadRepository = propiedadRepository;
    }

    public async Task<List<AgenteViewModel>> GetAllAgentesAsync(bool soloActivos = true)
    {
       
        var agentes = await _userManager.GetUsersInRoleAsync(ROLE_AGENTE);

   
        var agentesFiltrados = soloActivos
            ? agentes.Where(a => a.EsActivo).ToList()
            : agentes.ToList();

        agentesFiltrados = agentesFiltrados
            .OrderBy(a => a.Nombre)
            .ThenBy(a => a.Apellido)
            .ToList();

        var agentesViewModel = new List<AgenteViewModel>();

        foreach (var agente in agentesFiltrados)
        {
            var cantidadPropiedades = await GetCantidadPropiedadesByAgenteIdAsync(agente.Id);

            agentesViewModel.Add(new AgenteViewModel
            {
                Id = agente.Id,
                Nombre = agente.Nombre,
                Apellido = agente.Apellido,
                NombreCompleto = agente.NombreCompleto,
                Email = agente.Email ?? string.Empty,
                Telefono = agente.PhoneNumber,
                UrlImagenPerfil = agente.UrlImagenPerfil,
                CantidadPropiedades = cantidadPropiedades,
                EsActivo = agente.EsActivo
            });
        }

        return agentesViewModel;
    }

    public async Task<AgenteViewModel?> GetAgenteByIdAsync(string id)
    {
        var agente = await _userManager.FindByIdAsync(id);

        if (agente == null)
            return null;

        var cantidadPropiedades = await GetCantidadPropiedadesByAgenteIdAsync(agente.Id);

        return new AgenteViewModel
        {
            Id = agente.Id,
            Nombre = agente.Nombre,
            Apellido = agente.Apellido,
            NombreCompleto = agente.NombreCompleto,
            Email = agente.Email ?? string.Empty,
            Telefono = agente.PhoneNumber,
            UrlImagenPerfil = agente.UrlImagenPerfil,
            CantidadPropiedades = cantidadPropiedades,
            EsActivo = agente.EsActivo
        };
    }

    public async Task<List<AgenteViewModel>> GetAgentesByNombreAsync(string nombre)
    {
       
        var todosAgentes = await _userManager.GetUsersInRoleAsync(ROLE_AGENTE);

     
        var agentesFiltrados = todosAgentes
            .Where(a => a.EsActivo &&
                       (a.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                        a.Apellido.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                        a.NombreCompleto.Contains(nombre, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(a => a.Nombre)
            .ThenBy(a => a.Apellido)
            .ToList();

        var agentesViewModel = new List<AgenteViewModel>();

        foreach (var agente in agentesFiltrados)
        {
            var cantidadPropiedades = await GetCantidadPropiedadesByAgenteIdAsync(agente.Id);

            agentesViewModel.Add(new AgenteViewModel
            {
                Id = agente.Id,
                Nombre = agente.Nombre,
                Apellido = agente.Apellido,
                NombreCompleto = agente.NombreCompleto,
                Email = agente.Email ?? string.Empty,
                Telefono = agente.PhoneNumber,
                UrlImagenPerfil = agente.UrlImagenPerfil,
                CantidadPropiedades = cantidadPropiedades,
                EsActivo = agente.EsActivo
            });
        }

        return agentesViewModel;
    }

    public async Task<int> GetCantidadPropiedadesByAgenteIdAsync(string agenteId)
    {
        var propiedades = await _propiedadRepository.GetByAgenteIdAsync(agenteId);
        return propiedades.Count;
    }
}
