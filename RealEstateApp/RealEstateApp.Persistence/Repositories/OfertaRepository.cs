using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class OfertaRepository : GenericRepositoryAsync<Oferta>, IOfertaRepository
{
    public OfertaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Oferta>> GetByClienteAsync(string clienteId)
    {
        return await _context.Ofertas
            .Include(o => o.Propiedad)
            .Where(o => o.ClienteId == clienteId)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Oferta>> GetByAgenteAsync(string agenteId)
    {
        return await _context.Ofertas
            .Include(o => o.Propiedad)
            .Where(o => o.Propiedad.AgenteId == agenteId)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Oferta>> GetByPropiedadAsync(int propiedadId)
    {
        return await _context.Ofertas
            .Include(o => o.Propiedad)
            .Where(o => o.PropiedadId == propiedadId)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Oferta>> GetByClienteAndPropiedadAsync(string clienteId, int propiedadId)
    {
        return await _context.Ofertas
            .Include(o => o.Propiedad)
            .Where(o => o.ClienteId == clienteId && o.PropiedadId == propiedadId)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<bool> TieneOfertasPendientesAsync(string clienteId, int propiedadId)
    {
        return await _context.Ofertas
            .AnyAsync(o => o.ClienteId == clienteId &&
                          o.PropiedadId == propiedadId &&
                          o.Estado == EstadoOferta.Pendiente);
    }

    public async Task<bool> TieneOfertasAceptadasAsync(string clienteId, int propiedadId)
    {
        return await _context.Ofertas
            .AnyAsync(o => o.ClienteId == clienteId &&
                          o.PropiedadId == propiedadId &&
                          o.Estado == EstadoOferta.Aceptada);
    }

    public async Task<int> GetCantidadOfertasByAgenteAsync(string agenteId)
    {
        return await _context.Ofertas
            .Where(o => o.Propiedad.AgenteId == agenteId)
            .CountAsync();
    }

    public async Task<bool> HasAcceptedOfertaAsync(int propiedadId)
    {
        return await _context.Ofertas
            .AnyAsync(o => o.PropiedadId == propiedadId && 
                          o.Estado == EstadoOferta.Aceptada);
    }
}
