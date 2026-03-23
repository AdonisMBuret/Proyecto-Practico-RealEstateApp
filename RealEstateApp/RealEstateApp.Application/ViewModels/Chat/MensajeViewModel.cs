using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Chat;


public class MensajeViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Propiedad")]
    public int PropiedadId { get; set; }
    
    [Display(Name = "Código de Propiedad")]
    public string CodigoPropiedad { get; set; } = string.Empty;
    
    [Display(Name = "Emisor")]
    public string EmisorId { get; set; } = string.Empty;
    
    [Display(Name = "Nombre del Emisor")]
    public string EmisorNombre { get; set; } = string.Empty;
    
    [Display(Name = "Receptor")]
    public string ReceptorId { get; set; } = string.Empty;
    
    [Display(Name = "Nombre del Receptor")]
    public string ReceptorNombre { get; set; } = string.Empty;
    
    [Display(Name = "Contenido")]
    public string Contenido { get; set; } = string.Empty;
    
    [Display(Name = "Fecha de Envío")]
    public DateTime FechaEnvio { get; set; }
    
    [Display(Name = "Leído")]
    public bool EsLeido { get; set; }
    
    // Propiedades auxiliares para la vista
    public bool EsMio { get; set; }
    public string FechaFormateada => FechaEnvio.ToString("dd/MM/yyyy HH:mm");
}
