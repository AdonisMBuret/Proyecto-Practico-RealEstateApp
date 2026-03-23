using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Identity.Entities;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

   
    public async Task<List<string>> GetAgenteActivosIdsAsync()
    {
        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        return agentes
            .Where(u => u.EsActivo)
            .Select(u => u.Id)
            .ToList();
    }

    public async Task<bool> UpdateAgenteAsync(string agenteId, string nombre, string apellido, string telefono, string? urlImagen)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return false;

        var agente = await _userManager.FindByIdAsync(agenteId);
        
        if (agente == null)
            return false;

        agente.Nombre = nombre;
        agente.Apellido = apellido;
        agente.PhoneNumber = telefono;
        
        if (!string.IsNullOrWhiteSpace(urlImagen))
        {
            agente.UrlImagenPerfil = urlImagen;
        }

        var result = await _userManager.UpdateAsync(agente);
        return result.Succeeded;
    }

    public async Task<bool> ExisteAgenteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var agente = await _userManager.FindByIdAsync(id);
        if (agente == null || !agente.EsActivo)
            return false;

        return await _userManager.IsInRoleAsync(agente, "Agente");
    }

    public async Task<bool> IsAgenteActivoAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var user = await _userManager.FindByIdAsync(id);
        
        if (user == null || !user.EsActivo)
            return false;

        return await _userManager.IsInRoleAsync(user, "Agente");
    }

    public async Task<int> GetCantidadPropiedadesByAgenteAsync(string agenteId)
    {
        return await _context.Propiedades
            .CountAsync(p => p.AgenteId == agenteId);
    }

    public async Task<List<string>> GetAgentesByNombreIdsAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new List<string>();

        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        
        return agentes
            .Where(u => u.EsActivo && 
                       (u.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                        u.Apellido.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                        $"{u.Nombre} {u.Apellido}".Contains(nombre, StringComparison.OrdinalIgnoreCase)))
            .Select(u => u.Id)
            .ToList();
    }

    public async Task<(string Id, string Nombre, string Apellido, string Email, string? Telefono, string? UrlImagen)> GetAgentePerfilAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return default;

        var agente = await _userManager.FindByIdAsync(agenteId);
        
        if (agente == null)
            return default;

        var esAgente = await _userManager.IsInRoleAsync(agente, "Agente");
        
        if (!esAgente)
            return default;

        return (agente.Id, agente.Nombre, agente.Apellido, agente.Email!, agente.PhoneNumber, agente.UrlImagenPerfil);
    }

    public async Task<(string Id, string Nombre, string Apellido, string Email, string? Telefono, string? UrlImagen)> GetUsuarioPerfilAsync(string usuarioId)
    {
        if (string.IsNullOrWhiteSpace(usuarioId))
            return default;

        var usuario = await _userManager.FindByIdAsync(usuarioId);
        
        if (usuario == null)
            return default;

        return (usuario.Id, usuario.Nombre, usuario.Apellido, usuario.Email!, usuario.PhoneNumber, usuario.UrlImagenPerfil);
    }

    public async Task<bool> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var user = await _userManager.FindByIdAsync(id);
        return user != null;
    }

    public async Task<bool> EsAgenteActivoAsync(string id)
    {
        return await IsAgenteActivoAsync(id);
    }
    public async Task<object?> GetUsuarioByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return await _userManager.FindByIdAsync(id);
    }
    public async Task<List<AgenteViewModel>> GetAgenteActivosAsync()
    {
        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        
        var agentesActivos = agentes.Where(u => u.EsActivo).OrderBy(u => u.Nombre).ToList();

        var agentesViewModel = new List<AgenteViewModel>();

        foreach (var agente in agentesActivos)
        {
            var cantidadPropiedades = await GetCantidadPropiedadesByAgenteAsync(agente.Id);
            
            agentesViewModel.Add(new AgenteViewModel
            {
                Id = agente.Id,
                Nombre = agente.Nombre,
                Apellido = agente.Apellido,
                NombreCompleto = $"{agente.Nombre} {agente.Apellido}",
                Email = agente.Email!,
                Telefono = agente.PhoneNumber,
                UrlImagenPerfil = agente.UrlImagenPerfil,
                EsActivo = agente.EsActivo,
                CantidadPropiedades = cantidadPropiedades
            });
        }

        return agentesViewModel;
    }

    public async Task<AgenteViewModel?> GetAgenteByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var agente = await _userManager.FindByIdAsync(id);
        
        if (agente == null || !agente.EsActivo)
            return null;

        var esAgente = await _userManager.IsInRoleAsync(agente, "Agente");
        
        if (!esAgente)
            return null;

        var cantidadPropiedades = await GetCantidadPropiedadesByAgenteAsync(agente.Id);

        return new AgenteViewModel
        {
            Id = agente.Id,
            Nombre = agente.Nombre,
            Apellido = agente.Apellido,
            NombreCompleto = $"{agente.Nombre} {agente.Apellido}",
            Email = agente.Email!,
            Telefono = agente.PhoneNumber,
            UrlImagenPerfil = agente.UrlImagenPerfil,
            EsActivo = agente.EsActivo,
            CantidadPropiedades = cantidadPropiedades
        };
    }

    public async Task<List<AgenteViewModel>> GetAgentesByNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new List<AgenteViewModel>();

        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        
        var agentesFiltrados = agentes
            .Where(u => u.EsActivo && 
                       (u.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                        u.Apellido.Contains(nombre, StringComparison.OrdinalIgnoreCase) ||
                        $"{u.Nombre} {u.Apellido}".Contains(nombre, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(u => u.Nombre)
            .ToList();

        var agentesViewModel = new List<AgenteViewModel>();

        foreach (var agente in agentesFiltrados)
        {
            var cantidadPropiedades = await GetCantidadPropiedadesByAgenteAsync(agente.Id);
            
            agentesViewModel.Add(new AgenteViewModel
            {
                Id = agente.Id,
                Nombre = agente.Nombre,
                Apellido = agente.Apellido,
                NombreCompleto = $"{agente.Nombre} {agente.Apellido}",
                Email = agente.Email!,
                Telefono = agente.PhoneNumber,
                UrlImagenPerfil = agente.UrlImagenPerfil,
                EsActivo = agente.EsActivo,
                CantidadPropiedades = cantidadPropiedades
            });
        }

        return agentesViewModel;
    }

    public async Task<AgentePerfilViewModel?> GetAgentePerfilViewModelAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return null;

        var agente = await _userManager.FindByIdAsync(agenteId);
        
        if (agente == null)
            return null;

        var esAgente = await _userManager.IsInRoleAsync(agente, "Agente");
        
        if (!esAgente)
            return null;

        return new AgentePerfilViewModel
        {
            Id = agente.Id,
            Nombre = agente.Nombre,
            Apellido = agente.Apellido,
            Email = agente.Email!,
            Telefono = agente.PhoneNumber,
            Foto = agente.UrlImagenPerfil
        };
    }

    public async Task<bool> UpdateAgenteViewModelAsync(string agenteId, EditarAgenteViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(agenteId) || viewModel == null)
            return false;

        var agente = await _userManager.FindByIdAsync(agenteId);
        
        if (agente == null)
            return false;

        agente.Nombre = viewModel.Nombre;
        agente.Apellido = viewModel.Apellido;
        agente.PhoneNumber = viewModel.Telefono;
        
        
        if (!string.IsNullOrWhiteSpace(viewModel.FotoActual))
        {
            agente.UrlImagenPerfil = viewModel.FotoActual;
        }

        var result = await _userManager.UpdateAsync(agente);
        return result.Succeeded;
    }

    private async Task<int> GetCantidadMensajesAgenteAsync(string agenteId)
    {
        return await _context.Set<Mensaje>()
            .CountAsync(m => m.ReceptorId == agenteId);
    }

    private async Task<int> GetCantidadOfertasAgenteAsync(string agenteId)
    {
         return await _context.Set<Oferta>()
            .Where(o => _context.Propiedades.Any(p => p.AgenteId == agenteId && p.Id == o.PropiedadId))
            .CountAsync();
    }
}