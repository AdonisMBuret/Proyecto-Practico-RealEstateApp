using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface IMensajeRepository : IRepositoryAsync<Mensaje>
{
    Task<List<Mensaje>> GetMensajesByConversacionAsync(int propiedadId, string clienteId, string agenteId);
    Task<List<Chat>> GetConversacionesByAgenteAsync(string agenteId);
    Task<int> GetCantidadMensajesNoLeidosAsync(string usuarioId);
    Task MarcarComoLeidoAsync(int mensajeId);
}
