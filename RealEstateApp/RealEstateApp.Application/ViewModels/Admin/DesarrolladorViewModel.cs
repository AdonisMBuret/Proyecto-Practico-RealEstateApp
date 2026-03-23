using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Admin;


public class DesarrolladorViewModel
{
    public string Id { get; set; } = string.Empty;
    
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;
    
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;
    
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "Usuario")]
    public string UserName { get; set; } = string.Empty;
    
    [Display(Name = "Estado")]
    public bool IsActive { get; set; }
    
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaCreacion { get; set; }
    
 
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string EstadoTexto => IsActive ? "Activo" : "Inactivo";
}