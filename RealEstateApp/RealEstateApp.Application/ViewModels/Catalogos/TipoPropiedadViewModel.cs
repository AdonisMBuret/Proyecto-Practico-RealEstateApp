using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Catalogos;


public class TipoPropiedadViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;
    
    [Display(Name = "Cantidad de Propiedades")]
    public int CantidadPropiedades { get; set; }
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }
}