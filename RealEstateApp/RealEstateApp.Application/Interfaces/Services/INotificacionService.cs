using RealEstateApp.Application.ViewModels.Notificaciones;

namespace RealEstateApp.Application.Interfaces.Services;


public interface INotificacionService
{
    
    Task<NotificacionesResumenViewModel> GetResumenNotificacionesAgenteAsync(string agenteId);
    
    
    Task<List<NotificacionViewModel>> GetNotificacionesAgenteAsync(string agenteId);
    
    
    Task MarcarComoLeidaAsync(int notificacionId);
   
    Task MarcarTodasComoLeidasAsync(string agenteId);
}
