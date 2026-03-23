using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Account;

public class RegistroViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;
    
    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = null!;
    
    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    [Display(Name = "Teléfono")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono debe tener 10 dígitos")]
    public string Telefono { get; set; } = null!;
    
    [Display(Name = "Foto de Perfil")]
    public IFormFile? FotoPerfil { get; set; }
    
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "El nombre de usuario solo puede contener letras, números y guiones bajos")]
    public string NombreUsuario { get; set; } = null!;
    
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", 
        ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula y un número")]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
    [Display(Name = "Tipo de Usuario")]
    public string TipoUsuario { get; set; } = null!; 
}
