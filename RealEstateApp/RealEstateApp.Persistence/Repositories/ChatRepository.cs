using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Repositories;

public class ChatRepository : GenericRepositoryAsync<Chat>, IChatRepository
{
    public ChatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Chat?> GetByPropiedadAndClienteAsync(int propiedadId, string clienteId)
    {
        return await _context.Chats
            .FirstOrDefaultAsync(c => c.PropiedadId == propiedadId && c.ClienteId == clienteId);
    }

    public async Task<List<Chat>> GetByPropiedadIdAsync(int propiedadId)
    {
        return await _context.Chats
            .Where(c => c.PropiedadId == propiedadId)
            .ToListAsync();
    }

    public async Task<Chat?> GetByIdWithMensajesAsync(int chatId)
    {
        return await _context.Chats
            .Include(c => c.Mensajes.OrderBy(m => m.FechaEnvio))
            .Include(c => c.Propiedad)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    public async Task<Chat> GetOrCreateChatAsync(int propiedadId, string emisorId, string receptorId)
    {
        
        var chat = await _context.Chats
            .FirstOrDefaultAsync(c => c.PropiedadId == propiedadId &&
                                     ((c.ClienteId == emisorId && c.AgenteId == receptorId) ||
                                      (c.ClienteId == receptorId && c.AgenteId == emisorId)));

        if (chat == null)
        {
            
            chat = new Chat
            {
                PropiedadId = propiedadId,
                ClienteId = emisorId,
                AgenteId = receptorId,
                FechaCreacion = DateTime.UtcNow
            };

            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        return chat;
    }
}
