using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Agentes;


public class EditarAgenteViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [Display(Name = "Teléfono")]
    [RegularExpression(@"^(\+?1-?)?(\(?[0-9]{3}\)?[-.\s]?)?[0-9]{3}[-.\s]?[0-9]{4}$", 
        ErrorMessage = "Formato de teléfono inválido. Use: 809-123-4567")]
    public string Telefono { get; set; } = string.Empty;
    
  
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
   
    [Display(Name = "Foto Actual")]
    public string? FotoActual { get; set; }
    
 
    [Display(Name = "Nueva Foto de Perfil")]
    public IFormFile? NuevaFoto { get; set; }
}