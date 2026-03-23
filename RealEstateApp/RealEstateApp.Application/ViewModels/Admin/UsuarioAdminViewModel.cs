using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Admin;

public class UsuarioAdminViewModel
{
    public string Id { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es requerido")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;
    
    public string NombreCompleto => $"{Nombre} {Apellido}";
    
    [Required(ErrorMessage = "La cédula es requerida")]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;
    
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    
    public bool EsActivo { get; set; } = true;
    
    public string Rol { get; set; } = string.Empty;
}
