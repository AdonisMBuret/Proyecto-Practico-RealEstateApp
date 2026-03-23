using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Propiedades;


public class FiltrosPropiedadesViewModel
{
    [Display(Name = "Código de Propiedad")]
    public string? CodigoPropiedad { get; set; }
    
    [Display(Name = "Tipo de Propiedad")]
    public int? TipoPropiedadId { get; set; }
    
    [Display(Name = "Precio Mínimo")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo debe ser mayor o igual a 0")]
    public decimal? PrecioMinimo { get; set; }
    
    [Display(Name = "Precio Máximo")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio máximo debe ser mayor o igual a 0")]
    public decimal? PrecioMaximo { get; set; }
    
    [Display(Name = "Cantidad de Habitaciones")]
    [Range(0, 50, ErrorMessage = "La cantidad de habitaciones debe estar entre 0 y 50")]
    public int? CantidadHabitaciones { get; set; }
    
    [Display(Name = "Cantidad de Baños")]
    [Range(0, 20, ErrorMessage = "La cantidad de baños debe estar entre 0 y 20")]
    public int? CantidadBanos { get; set; }
    

    public string? OrdenarPor { get; set; } = "FechaCreacion"; 
    
    public bool Descendente { get; set; } = true; 
    
 
    public bool TieneFiltros()
    {
        return !string.IsNullOrWhiteSpace(CodigoPropiedad) ||
               TipoPropiedadId.HasValue ||
               PrecioMinimo.HasValue ||
               PrecioMaximo.HasValue ||
               CantidadHabitaciones.HasValue ||
               CantidadBanos.HasValue;
    }
    
    
    public bool EsRangoPrecioValido()
    {
        if (!PrecioMinimo.HasValue && !PrecioMaximo.HasValue)
            return true;
            
        if (PrecioMinimo.HasValue && PrecioMaximo.HasValue)
            return PrecioMinimo.Value <= PrecioMaximo.Value;
            
        return true;
    }
}
