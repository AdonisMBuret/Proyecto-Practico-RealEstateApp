using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Admin;

public class SaveAdministradorViewModel : IValidatableObject
{
    public string? Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La cédula es requerida")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "La cédula debe tener exactamente 11 dígitos")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "La cédula debe contener solo números")]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El usuario es requerido")]
    [StringLength(20, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 20 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "El usuario solo puede contener letras, números, puntos, guiones y guiones bajos")]
    [Display(Name = "Usuario")]
    public string UserName { get; set; } = string.Empty;
    
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string? Password { get; set; } = string.Empty;
    
    
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    public string? ConfirmPassword { get; set; } = string.Empty;
    
    public bool IsEdit => !string.IsNullOrEmpty(Id);
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

      
        if (!IsEdit)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                results.Add(new ValidationResult(
                    "La contraseña es requerida", 
                    new[] { nameof(Password) }));
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                results.Add(new ValidationResult(
                    "La confirmación de contraseña es requerida", 
                    new[] { nameof(ConfirmPassword) }));
            }
        }

        if (!string.IsNullOrWhiteSpace(Password) || !string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            if (Password != ConfirmPassword)
            {
                results.Add(new ValidationResult(
                    "Las contraseñas no coinciden", 
                    new[] { nameof(ConfirmPassword) }));
            }
        }

        return results;
    }
}