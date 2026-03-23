using Microsoft.EntityFrameworkCore;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;
using AutoMapper;

namespace RealEstateApp.Persistence.Repositories;

public class PropiedadRepository : GenericRepositoryAsync<Propiedad>, IPropiedadRepository
{
    private readonly IMapper _mapper;

    public PropiedadRepository(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<List<Propiedad>> GetAllDisponiblesAsync()
    {
        return await _context.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadesMejoras)
                .ThenInclude(pm => pm.Mejora)
            .Where(p => p.Estado == EstadoPropiedad.Disponible)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Propiedad>> GetAllWithDetailsAsync()
    {
        return await GetAllDisponiblesAsync();
    }

    public async Task<Propiedad?> GetByIdWithDetailsAsync(int id)
    {
        return await GetDetalleByIdAsync(id);
    }

    public async Task<Propiedad?> GetByCodigoAsync(string codigo)
    {
        return await _context.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadesMejoras)
                .ThenInclude(pm => pm.Mejora)
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<List<Propiedad>> GetByFiltrosAsync(int? tipoPropiedadId = null, decimal? precioMin = null, decimal? precioMax = null, int? habitaciones = null, int? banos = null)
    {
        var query = _context.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Imagenes)
            .Where(p => p.Estado == EstadoPropiedad.Disponible);

        if (tipoPropiedadId.HasValue)
            query = query.Where(p => p.TipoPropiedadId == tipoPropiedadId.Value);

        if (precioMin.HasValue)
            query = query.Where(p => p.Precio >= precioMin.Value);

        if (precioMax.HasValue)
            query = query.Where(p => p.Precio <= precioMax.Value);

        if (habitaciones.HasValue)
            query = query.Where(p => p.CantidadHabitaciones >= habitaciones.Value);

        if (banos.HasValue)
            query = query.Where(p => p.CantidadBanos >= banos.Value);

        return await query
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Propiedad?> GetDetalleByIdAsync(int id)
    {
        return await _context.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadesMejoras)
                .ThenInclude(pm => pm.Mejora)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Propiedad>> GetByAgenteIdAsync(string agenteId, bool soloDisponibles = true)
    {
        var query = _context.Propiedades
            .Include(p => p.TipoPropiedad)
            .Include(p => p.TipoVenta)
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadesMejoras)
                .ThenInclude(pm => pm.Mejora)
            .Where(p => p.AgenteId == agenteId);

        if (soloDisponibles)
            query = query.Where(p => p.Estado == EstadoPropiedad.Disponible);

        return await query
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<int> GetCantidadByAgenteAsync(string agenteId)
    {
        return await _context.Propiedades
            .CountAsync(p => p.AgenteId == agenteId);
    }

    public async Task<bool> EstaDisponibleAsync(int id)
    {
        return await _context.Propiedades
            .AnyAsync(p => p.Id == id && p.Estado == EstadoPropiedad.Disponible);
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
    {
        var query = _context.Propiedades.Where(p => p.Codigo == codigo);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        
        return await query.AnyAsync();
    }

    public async Task<string> GenerarCodigoAsync()
    {
        var ultimoNumero = await _context.Propiedades
            .Where(p => p.Codigo.StartsWith("PROP"))
            .Select(p => p.Codigo)
            .OrderByDescending(c => c)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(ultimoNumero))
            return "PROP001";

        var numeroStr = ultimoNumero.Substring(4);
        if (int.TryParse(numeroStr, out int numero))
        {
            numero++;
            return $"PROP{numero:D3}";
        }

        return "PROP001";
    }

    public async Task<(int Disponibles, int Vendidas)> GetEstadisticasAsync()
    {
        var disponibles = await _context.Propiedades
            .CountAsync(p => p.Estado == EstadoPropiedad.Disponible);

        var vendidas = await _context.Propiedades
            .CountAsync(p => p.Estado == EstadoPropiedad.Vendida);

        return (disponibles, vendidas);
    }

    public async Task DeleteAllByAgenteAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return;

        var propiedades = await _context.Propiedades
            .Include(p => p.Imagenes)
            .Include(p => p.PropiedadesMejoras)
            .Include(p => p.PropiedadesFavoritas)
            .Include(p => p.Ofertas)
            .Include(p => p.Chats)
                .ThenInclude(c => c.Mensajes)
            .Where(p => p.AgenteId == agenteId)
            .ToListAsync();

        if (!propiedades.Any())
            return;

        foreach (var propiedad in propiedades)
        {
            if (propiedad.Chats?.Any() == true)
            {
                foreach (var chat in propiedad.Chats)
                {
                    if (chat.Mensajes?.Any() == true)
                    {
                        _context.Mensajes.RemoveRange(chat.Mensajes);
                    }
                }
                _context.Chats.RemoveRange(propiedad.Chats);
            }

            if (propiedad.Ofertas?.Any() == true)
            {
                _context.Ofertas.RemoveRange(propiedad.Ofertas);
            }

            if (propiedad.PropiedadesFavoritas?.Any() == true)
            {
                _context.PropiedadesFavoritas.RemoveRange(propiedad.PropiedadesFavoritas);
            }

            if (propiedad.PropiedadesMejoras?.Any() == true)
            {
                _context.PropiedadesMejoras.RemoveRange(propiedad.PropiedadesMejoras);
            }

            if (propiedad.Imagenes?.Any() == true)
            {
                _context.ImagenesPropiedades.RemoveRange(propiedad.Imagenes);
            }
        }

        _context.Propiedades.RemoveRange(propiedades);

        await _context.SaveChangesAsync();
    }

    public async Task AddPropiedadMejoraAsync(PropiedadMejora propiedadMejora)
    {
        await _context.PropiedadesMejoras.AddAsync(propiedadMejora);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePropiedadMejorasAsync(int propiedadId)
    {
        var mejorasExistentes = await _context.PropiedadesMejoras
            .Where(pm => pm.PropiedadId == propiedadId)
            .ToListAsync();
        
        if (mejorasExistentes.Any())
        {
            _context.PropiedadesMejoras.RemoveRange(mejorasExistentes);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<PropiedadViewModel>> GetAllDisponiblesViewModelAsync()
    {
        var propiedades = await GetAllDisponiblesAsync();
        return _mapper.Map<List<PropiedadViewModel>>(propiedades);
    }

    public async Task<PropiedadViewModel?> GetByCodigoViewModelAsync(string codigo)
    {
        var propiedad = await GetByCodigoAsync(codigo);
        return propiedad != null ? _mapper.Map<PropiedadViewModel>(propiedad) : null;
    }

    public async Task<List<PropiedadViewModel>> GetByFiltrosViewModelAsync(FiltrosPropiedadesViewModel filtros)
    {
        var propiedades = await GetByFiltrosAsync(
            filtros.TipoPropiedadId, 
            filtros.PrecioMinimo, 
            filtros.PrecioMaximo, 
            filtros.CantidadHabitaciones, 
            filtros.CantidadBanos);

        return _mapper.Map<List<PropiedadViewModel>>(propiedades);
    }

    public async Task<PropiedadDetalleViewModel?> GetDetalleViewModelByIdAsync(int id)
    {
        var propiedad = await GetDetalleByIdAsync(id);
        return propiedad != null ? _mapper.Map<PropiedadDetalleViewModel>(propiedad) : null;
    }

    public async Task<List<PropiedadViewModel>> GetByAgenteIdViewModelAsync(string agenteId, bool soloDisponibles = true)
    {
        var propiedades = await GetByAgenteIdAsync(agenteId, soloDisponibles);
        return _mapper.Map<List<PropiedadViewModel>>(propiedades);
    }
}
