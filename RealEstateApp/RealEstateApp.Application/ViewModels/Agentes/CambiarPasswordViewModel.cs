using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Agentes;


public class CambiarPasswordViewModel
{
    [Required(ErrorMessage = "La contraseña actual es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña Actual")]
    public string PasswordActual { get; set; } = null!;

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    public string PasswordNuevo { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    [Compare("PasswordNuevo", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarPasswordNuevo { get; set; } = null!;
}
