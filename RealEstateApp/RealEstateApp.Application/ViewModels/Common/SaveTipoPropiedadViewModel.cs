using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Common;

public class SaveTipoPropiedadViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = null!;

    [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
}
