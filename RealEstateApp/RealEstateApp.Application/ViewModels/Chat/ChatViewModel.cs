using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Chat;


public class ChatViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Propiedad")]
    public int PropiedadId { get; set; }
    
    [Display(Name = "Emisor")]
    public string EmisorId { get; set; } = string.Empty;
    
    [Display(Name = "Receptor")]
    public string ReceptorId { get; set; } = string.Empty;
    
    [Display(Name = "Contenido")]
    public string Contenido { get; set; } = string.Empty;
    
    [Display(Name = "Fecha de Envío")]
    public DateTime FechaEnvio { get; set; }
    
    [Display(Name = "Leído")]
    public bool EsLeido { get; set; }
}
