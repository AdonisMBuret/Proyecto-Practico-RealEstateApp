namespace RealEstateApp.Application.ViewModels.Notificaciones;


public class NotificacionesResumenViewModel
{
    public int TotalNoLeidas { get; set; }
    public int MensajesNuevos { get; set; }
    public int OfertasNuevas { get; set; }
    public List<NotificacionViewModel> UltimasNotificaciones { get; set; } = new();
    
    public bool TieneNotificaciones => TotalNoLeidas > 0;
}


public class NotificacionViewModel
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; 
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty; 
    public string Url { get; set; } = string.Empty; 
    public DateTime FechaCreacion { get; set; }
    public bool EsLeida { get; set; }
    
    public string TiempoTranscurrido
    {
        get
        {
            var diferencia = DateTime.UtcNow - FechaCreacion;
            
            if (diferencia.TotalMinutes < 1)
                return "Ahora mismo";
            if (diferencia.TotalMinutes < 60)
                return $"Hace {(int)diferencia.TotalMinutes} minutos";
            if (diferencia.TotalHours < 24)
                return $"Hace {(int)diferencia.TotalHours} horas";
            if (diferencia.TotalDays < 7)
                return $"Hace {(int)diferencia.TotalDays} días";
            
            return FechaCreacion.ToString("dd/MM/yyyy");
        }
    }
    
    public string ClaseCss => Tipo switch
    {
        "Mensaje" => "notification-mensaje",
        "Oferta" => "notification-oferta",
        _ => "notification-sistema"
    };
}
