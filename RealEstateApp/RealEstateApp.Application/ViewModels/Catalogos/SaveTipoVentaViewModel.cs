using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Catalogos;

public class SaveTipoVentaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    [Display(Name = "Nombre del Tipo")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;
}