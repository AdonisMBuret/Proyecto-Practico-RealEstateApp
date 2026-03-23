using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Propiedades;


public class PropiedadViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;
    
    [Display(Name = "Tipo de Propiedad")]
    public string TipoPropiedad { get; set; } = string.Empty;
    
    public int TipoPropiedadId { get; set; }
    
    [Display(Name = "Tipo de Venta")]
    public string TipoVenta { get; set; } = string.Empty;
    
    public int TipoVentaId { get; set; }
    
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
    
    [Display(Name = "Imagen Principal")]
    public string? ImagenPrincipal { get; set; }
    
    [Display(Name = "Agente")]
    public string AgenteNombre { get; set; } = string.Empty;
    
   
    [Display(Name = "Agente")]
    public string NombreAgente => AgenteNombre;
    
    public string AgenteId { get; set; } = string.Empty;
    
    [Display(Name = "Teléfono del Agente")]
    public string AgenteTelefono { get; set; } = string.Empty;
    
    [Display(Name = "Email del Agente")]
    public string AgenteEmail { get; set; } = string.Empty;
    
    [Display(Name = "Foto del Agente")]
    public string? AgenteFoto { get; set; }
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }
    
    [Display(Name = "Estado")]
    public string EstadoTexto { get; set; } = string.Empty;
    
   
    [Display(Name = "Mejoras")]
    public List<string> Mejoras { get; set; } = new List<string>();
    
    public bool EsReciente => (DateTime.UtcNow - FechaCreacion).TotalDays <= 7;
    
    public string PrecioFormateado => Precio.ToString("C0");
    
    public bool EsVendida => EstadoTexto == "Vendida";
}
