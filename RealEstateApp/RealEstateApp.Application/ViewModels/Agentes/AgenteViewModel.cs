using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Agentes;

public class AgenteViewModel
{
    public string Id { get; set; } = null!;
    
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = null!;
    
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = null!;
    
    [Display(Name = "Correo Electrónico")]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Display(Name = "Teléfono")]
    [Phone]
    public string? Telefono { get; set; }
    
    [Display(Name = "Foto de Perfil")]
    public string? UrlImagenPerfil { get; set; }
    
    [Display(Name = "Cantidad de Propiedades")]
    public int CantidadPropiedades { get; set; }
    
    [Display(Name = "Activo")]
    public bool EsActivo { get; set; }
}
