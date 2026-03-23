using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Chat;


public class SaveMensajeViewModel
{
    [Required(ErrorMessage = "La propiedad es requerida")]
    public int PropiedadId { get; set; }
    
    [Required(ErrorMessage = "El emisor es requerido")]
    public string EmisorId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El receptor es requerido")]
    public string ReceptorId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El contenido del mensaje es requerido")]
    [StringLength(1000, ErrorMessage = "El mensaje no puede exceder los 1000 caracteres")]
    [MinLength(5, ErrorMessage = "El mensaje debe tener al menos 5 caracteres")]
    public string Contenido { get; set; } = string.Empty;
}