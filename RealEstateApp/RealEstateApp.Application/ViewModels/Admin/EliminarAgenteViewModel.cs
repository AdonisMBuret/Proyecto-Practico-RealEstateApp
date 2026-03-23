using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Admin;


public class EliminarAgenteViewModel
{
    public string Id { get; set; } = string.Empty;
    
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto { get; set; } = string.Empty;
    
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "Cantidad de Propiedades")]
    public int CantidadPropiedades { get; set; }
}