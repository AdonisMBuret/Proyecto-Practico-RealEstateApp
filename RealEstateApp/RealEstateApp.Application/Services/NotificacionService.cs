using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Notificaciones;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Services;

public class NotificacionService : INotificacionService
{
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IOfertaRepository _ofertaRepository;
    private readonly ILogger<NotificacionService> _logger;

    public NotificacionService(
        IMensajeRepository mensajeRepository,
        IOfertaRepository ofertaRepository,
        ILogger<NotificacionService> logger)
    {
        _mensajeRepository = mensajeRepository;
        _ofertaRepository = ofertaRepository;
        _logger = logger;
    }

    public async Task<NotificacionesResumenViewModel> GetResumenNotificacionesAgenteAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return new NotificacionesResumenViewModel();

        try
        {
            var mensajesNoLeidos = await _mensajeRepository.GetCantidadMensajesNoLeidosAsync(agenteId);
            
            var ofertas = await _ofertaRepository.GetByAgenteAsync(agenteId);
            var ofertasPendientes = ofertas.Count(o => o.Estado == Domain.Enums.EstadoOferta.Pendiente);
            
            // Crear notificaciones recientes
            var notificaciones = new List<NotificacionViewModel>();
            
            if (mensajesNoLeidos > 0)
            {
                notificaciones.Add(new NotificacionViewModel
                {
                    Tipo = "Mensaje",
                    Titulo = "Mensajes nuevos",
                    Descripcion = $"Tienes {mensajesNoLeidos} mensaje(s) sin leer",
                    Icono = "bi-chat-dots-fill",
                    Url = "/Agente/Conversaciones",
                    FechaCreacion = DateTime.UtcNow,
                    EsLeida = false
                });
            }
            
            if (ofertasPendientes > 0)
            {
                notificaciones.Add(new NotificacionViewModel
                {
                    Tipo = "Oferta",
                    Titulo = "Ofertas pendientes",
                    Descripcion = $"Tienes {ofertasPendientes} oferta(s) pendiente(s) de revisar",
                    Icono = "bi-cash-stack",
                    Url = "/Agente/Ofertas",
                    FechaCreacion = DateTime.UtcNow,
                    EsLeida = false
                });
            }

            return new NotificacionesResumenViewModel
            {
                MensajesNuevos = mensajesNoLeidos,
                OfertasNuevas = ofertasPendientes,
                TotalNoLeidas = mensajesNoLeidos + ofertasPendientes,
                UltimasNotificaciones = notificaciones.Take(5).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de notificaciones para agente {AgenteId}", agenteId);
            return new NotificacionesResumenViewModel();
        }
    }

    public async Task<List<NotificacionViewModel>> GetNotificacionesAgenteAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return new List<NotificacionViewModel>();

        var resumen = await GetResumenNotificacionesAgenteAsync(agenteId);
        return resumen.UltimasNotificaciones;
    }

    public Task MarcarComoLeidaAsync(int notificacionId)
    {
        
        return Task.CompletedTask;
    }

    public async Task MarcarTodasComoLeidasAsync(string agenteId)
    {
        
        return;
    }
}
