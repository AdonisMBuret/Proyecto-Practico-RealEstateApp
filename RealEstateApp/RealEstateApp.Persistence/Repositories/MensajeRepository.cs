using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;


public class MensajeRepository : GenericRepositoryAsync<Mensaje>, IMensajeRepository
{
    public MensajeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Mensaje>> GetMensajesByConversacionAsync(int propiedadId, string clienteId, string agenteId)
    {
        return await _context.Mensajes
            .Include(m => m.Chat)
            .Where(m => m.Chat.PropiedadId == propiedadId &&
                       m.Chat.ClienteId == clienteId &&
                       m.Chat.AgenteId == agenteId)
            .OrderBy(m => m.FechaEnvio)
            .ToListAsync();
    }

    public async Task<List<Chat>> GetConversacionesByAgenteAsync(string agenteId)
    {
        return await _context.Chats
            .Include(c => c.Propiedad)
            .Include(c => c.Mensajes)
            .Where(c => c.AgenteId == agenteId)
            .OrderByDescending(c => c.Mensajes.Max(m => m.FechaEnvio))
            .ToListAsync();
    }

    public async Task<int> GetCantidadMensajesNoLeidosAsync(string usuarioId)
    {
        return await _context.Mensajes
            .Include(m => m.Chat)
            .CountAsync(m => (m.Chat.AgenteId == usuarioId || m.Chat.ClienteId == usuarioId) && 
                           m.ReceptorId == usuarioId && !m.EsLeido);
    }

    public async Task MarcarComoLeidoAsync(int mensajeId)
    {
        var mensaje = await _context.Mensajes.FindAsync(mensajeId);
        if (mensaje != null)
        {
            mensaje.EsLeido = true;
            await _context.SaveChangesAsync();
        }
    }
}
