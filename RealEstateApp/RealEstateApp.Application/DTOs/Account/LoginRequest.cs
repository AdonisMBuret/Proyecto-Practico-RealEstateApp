using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.DTOs.Account;

public class LoginRequest
{
    [Required(ErrorMessage = "El correo o nombre de usuario es requerido")]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; } = string.Empty;
}
