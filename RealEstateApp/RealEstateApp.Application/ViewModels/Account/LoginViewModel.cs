using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario o correo es requerido")]
    [Display(Name = "Usuario o Correo Electrónico")]
    public string UsuarioOCorreo { get; set; } = null!;
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;
    
    [Display(Name = "Recordarme")]
    public bool RecordarMe { get; set; }
    
    public string? ReturnUrl { get; set; }
}
