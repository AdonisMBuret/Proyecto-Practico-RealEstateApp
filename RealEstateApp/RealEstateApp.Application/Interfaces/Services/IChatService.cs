using RealEstateApp.Application.ViewModels.Chat;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IChatService
{
    Task<ChatViewModel> EnviarMensajeAsync(SaveMensajeViewModel viewModel);
    Task<List<ConversacionViewModel>> GetConversacionesByAgenteAsync(string agenteId);
    Task<List<MensajeViewModel>> GetMensajesByConversacionAsync(int propiedadId, string clienteId, string agenteId);
    Task MarcarComoLeidoAsync(int mensajeId);
}