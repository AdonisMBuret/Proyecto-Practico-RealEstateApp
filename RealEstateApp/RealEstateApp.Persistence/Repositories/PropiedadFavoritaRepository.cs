using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class PropiedadFavoritaRepository : GenericRepositoryAsync<PropiedadFavorita>, IFavoritoRepository
{
    public PropiedadFavoritaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> EsFavoritoAsync(string clienteId, int propiedadId)
    {
        return await _context.PropiedadesFavoritas
            .AnyAsync(pf => pf.ClienteId == clienteId && pf.PropiedadId == propiedadId);
    }

    public async Task<PropiedadFavorita?> GetByClienteYPropiedadAsync(string clienteId, int propiedadId)
    {
        return await _context.PropiedadesFavoritas
            .FirstOrDefaultAsync(pf => pf.ClienteId == clienteId && pf.PropiedadId == propiedadId);
    }

    public async Task<List<int>> GetPropiedadesFavoritasIdsAsync(string clienteId)
    {
        return await _context.PropiedadesFavoritas
            .Where(pf => pf.ClienteId == clienteId)
            .Select(pf => pf.PropiedadId)
            .ToListAsync();
    }

    public async Task<List<Propiedad>> GetPropiedadesFavoritasAsync(string clienteId)
    {
        return await _context.PropiedadesFavoritas
            .Where(pf => pf.ClienteId == clienteId)
            .Include(pf => pf.Propiedad)
            .ThenInclude(p => p.TipoPropiedad)
            .Include(pf => pf.Propiedad)
            .ThenInclude(p => p.TipoVenta)
            .Include(pf => pf.Propiedad)
            .ThenInclude(p => p.Imagenes)
            .Include(pf => pf.Propiedad)
            .ThenInclude(p => p.PropiedadesMejoras)
            .ThenInclude(pm => pm.Mejora)
            .Select(pf => pf.Propiedad)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<int> GetCantidadFavoritosAsync(string clienteId)
    {
        return await _context.PropiedadesFavoritas
            .CountAsync(pf => pf.ClienteId == clienteId);
    }
}
