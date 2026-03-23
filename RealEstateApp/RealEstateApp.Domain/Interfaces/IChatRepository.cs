using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface IChatRepository : IRepositoryAsync<Chat>
{
    Task<Chat?> GetByPropiedadAndClienteAsync(int propiedadId, string clienteId);
    Task<List<Chat>> GetByPropiedadIdAsync(int propiedadId);
    Task<Chat?> GetByIdWithMensajesAsync(int chatId);
    Task<Chat> GetOrCreateChatAsync(int propiedadId, string emisorId, string receptorId);
}
