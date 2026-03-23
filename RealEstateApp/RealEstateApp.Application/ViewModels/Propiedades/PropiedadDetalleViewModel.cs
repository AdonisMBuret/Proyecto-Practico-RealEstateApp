using System.ComponentModel.DataAnnotations;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.ViewModels.Propiedades;

public class PropiedadDetalleViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;
    
    [Display(Name = "Tipo de Propiedad")]
    public string TipoPropiedad { get; set; } = string.Empty;
    
    [Display(Name = "Tipo de Venta")]
    public string TipoVenta { get; set; } = string.Empty;
    
    [Display(Name = "Precio")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal Precio { get; set; }
    
    [Display(Name = "Habitaciones")]
    public int CantidadHabitaciones { get; set; }
    
    [Display(Name = "Baños")]
    public int CantidadBanos { get; set; }
    
    [Display(Name = "Tamaño (m²)")]
    public double TamanoEnMetros { get; set; }
    
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;
    
    [Display(Name = "Imágenes")]
    public List<string> Imagenes { get; set; } = new();
    
    [Display(Name = "Mejoras")]
    public List<string> Mejoras { get; set; } = new();
    
    [Display(Name = "Agente")]
    public AgenteInfoViewModel Agente { get; set; } = new();
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }

    public bool EsReciente => (DateTime.UtcNow - FechaCreacion).TotalDays <= 7;
    
    public bool EstaDisponible { get; set; } = true;
    public bool PuedeContactarAgente { get; set; } = true;
}