using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class TipoVentaRepository : GenericRepositoryAsync<TipoVenta>, ITipoVentaRepository
{
    public TipoVentaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TipoVenta?> GetByIdWithPropiedadesAsync(int id)
    {
        return await _context.TiposVentas
            .Include(tv => tv.Propiedades)
            .FirstOrDefaultAsync(tv => tv.Id == id);
    }

    public async Task<int> GetCantidadPropiedadesAsync(int tipoVentaId)
    {
        return await _context.Propiedades
            .CountAsync(p => p.TipoVentaId == tipoVentaId);
    }

    public async Task<bool> ExistsWithNameAsync(string nombre, int? excludeId = null)
    {
        var query = _context.TiposVentas.Where(tv => tv.Nombre.ToLower() == nombre.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(tv => tv.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}
