using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Agentes;

public class AgenteInfoViewModel
{
    public string Id { get; set; } = string.Empty;
    
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;
    
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Nombre} {Apellido}";
    
    [Display(Name = "Correo Electrónico")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "Teléfono")]
    [Phone]
    public string Telefono { get; set; } = string.Empty;
    
    [Display(Name = "Foto de Perfil")]
    public string? Foto { get; set; }
}