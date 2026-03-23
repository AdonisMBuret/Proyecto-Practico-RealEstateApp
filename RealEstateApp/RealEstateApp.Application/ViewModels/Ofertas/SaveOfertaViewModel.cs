using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstateApp.Application.ViewModels.Ofertas;

public class SaveOfertaViewModel
{
    [Required(ErrorMessage = "La propiedad es requerida")]
    public int PropiedadId { get; set; }
    
    [Required(ErrorMessage = "El cliente es requerido")]
    public string ClienteId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El monto de la oferta es requerido")]
    [Range(1000, 100000000, ErrorMessage = "El monto debe estar entre RD$1,000 y RD$100,000,000")]
    [Display(Name = "Monto de la Oferta (DOP)")]
    public decimal MontoOferta { get; set; }
    
    [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres")]
    [Display(Name = "Comentarios (Opcional)")]
    public string? Comentarios { get; set; }

   
    public string? MontoFormateado 
    { 
        get => MontoOferta.ToString("N0", CultureInfo.CreateSpecificCulture("es-DO"));
        set 
        {
            if (!string.IsNullOrEmpty(value))
            {
                var cleanValue = value.Replace(",", "").Replace(".", "").Replace(" ", "");
                if (decimal.TryParse(cleanValue, out var result))
                {
                    MontoOferta = result;
                }
            }
        }
    }
}