using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class TipoPropiedadRepository : GenericRepositoryAsync<TipoPropiedad>, ITipoPropiedadRepository
{
    public TipoPropiedadRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TipoPropiedad?> GetByIdWithPropiedadesAsync(int id)
    {
        return await _context.TiposPropiedades
            .Include(tp => tp.Propiedades)
            .FirstOrDefaultAsync(tp => tp.Id == id);
    }

    public async Task<bool> ExisteConNombreAsync(string nombre, int? excludeId = null)
    {
        var query = _context.TiposPropiedades.Where(tp => tp.Nombre.ToLower() == nombre.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(tp => tp.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<int> GetCantidadPropiedadesAsync(int tipoPropiedadId)
    {
        return await _context.Propiedades
            .CountAsync(p => p.TipoPropiedadId == tipoPropiedadId);
    }

    public async Task<List<TipoPropiedad>> GetTiposConPropiedadesAsync()
    {
        return await _context.TiposPropiedades
            .Include(tp => tp.Propiedades)
            .OrderBy(tp => tp.Nombre)
            .ToListAsync();
    }
}
