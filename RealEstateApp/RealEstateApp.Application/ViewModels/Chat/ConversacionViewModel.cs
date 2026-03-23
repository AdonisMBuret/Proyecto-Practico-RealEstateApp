using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Chat;

public class ConversacionViewModel
{
    [Display(Name = "Propiedad")]
    public int PropiedadId { get; set; }
    
    [Display(Name = "Código de Propiedad")]
    public string CodigoPropiedad { get; set; } = string.Empty;
    
    [Display(Name = "Cliente")]
    public string ClienteId { get; set; } = string.Empty;
    
    [Display(Name = "Nombre del Cliente")]
    public string ClienteNombre { get; set; } = string.Empty;
    
    [Display(Name = "Último Mensaje")]
    public string UltimoMensaje { get; set; } = string.Empty;
    
    [Display(Name = "Fecha del Último Mensaje")]
    public DateTime FechaUltimoMensaje { get; set; }
    
    public DateTime UltimoMensajeFecha => FechaUltimoMensaje;
    
    [Display(Name = "Mensajes no Leídos")]
    public int MensajesNoLeidos { get; set; }
    
    [Display(Name = "Total de Mensajes")]
    public int TotalMensajes { get; set; }
    
    public bool TieneMensajesNoLeidos => MensajesNoLeidos > 0;
    
    public bool TieneNuevosMensajes => TieneMensajesNoLeidos;
    
    public bool EsConversacionActiva { get; set; } = true;
    
    public string FechaFormateada => FechaUltimoMensaje.ToString("dd/MM/yyyy HH:mm");
}