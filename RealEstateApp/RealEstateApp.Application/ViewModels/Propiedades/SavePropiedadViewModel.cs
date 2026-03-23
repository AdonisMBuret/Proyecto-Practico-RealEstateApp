using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Propiedades;


public class SavePropiedadViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de propiedad")]
    [Display(Name = "Tipo de Propiedad")]
    public int TipoPropiedadId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de venta")]
    [Display(Name = "Tipo de Venta")]
    public int TipoVentaId { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
    [Display(Name = "Precio (DOP)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "El tamaño en metros es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tamaño debe ser mayor que 0")]
    [Display(Name = "Tamaño en Metros Cuadrados")]
    public int TamanoEnMetros { get; set; }

    [Range(0, 20, ErrorMessage = "La cantidad de habitaciones debe estar entre 0 y 20")]
    [Display(Name = "Cantidad de Habitaciones")]
    public int CantidadHabitaciones { get; set; }

    [Range(0, 10, ErrorMessage = "La cantidad de baños debe estar entre 0 y 10")]
    [Display(Name = "Cantidad de Baños")]
    public int CantidadBanos { get; set; }

    [Display(Name = "Mejoras")]
    public List<int> MejorasSeleccionadas { get; set; } = new();

    [Display(Name = "Imágenes de la Propiedad (1-4 imágenes)")]
    public List<IFormFile> Imagenes { get; set; } = new();

    public List<string> ImagenesActuales { get; set; } = new();
    
    public List<string> ImagenesAEliminar { get; set; } = new();

    public bool ValidarImagenes(bool esEdicion = false)
    {
        if (!esEdicion)
        {
            return Imagenes.Any() || ImagenesActuales.Any();
        }
        
        return true;
    }
    
    public bool ValidarCantidadImagenes()
    {
        var totalImagenes = (Imagenes?.Count ?? 0) + (ImagenesActuales?.Count ?? 0);
        return totalImagenes <= 4;
    }
}