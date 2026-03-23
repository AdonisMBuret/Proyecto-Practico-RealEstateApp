using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class MejoraRepository : GenericRepositoryAsync<Mejora>, IMejoraRepository
{
    public MejoraRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Mejora>> GetAllActiveAsync()
    {
        return await _context.Mejoras
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<List<Mejora>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Mejoras
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<int> GetCantidadPropiedadesAsync(int mejoraId)
    {
        return await _context.PropiedadesMejoras
            .Where(pm => pm.MejoraId == mejoraId)
            .CountAsync();
    }

    public async Task<bool> ExistsWithNameAsync(string nombre, int? excludeId = null)
    {
        var query = _context.Mejoras.Where(m => m.Nombre.ToLower() == nombre.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}
