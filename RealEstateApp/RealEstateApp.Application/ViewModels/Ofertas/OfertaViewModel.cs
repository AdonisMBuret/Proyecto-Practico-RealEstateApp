using System.ComponentModel.DataAnnotations;
using RealEstateApp.Domain.Enums;

namespace RealEstateApp.Application.ViewModels.Ofertas;


public class OfertaViewModel
{
    public int Id { get; set; }
    
    [Display(Name = "Propiedad")]
    public int PropiedadId { get; set; }
    
    [Display(Name = "Código de Propiedad")]
    public string CodigoPropiedad { get; set; } = string.Empty;
    
    [Display(Name = "Código de Propiedad")]
    public string? PropiedadCodigo { get; set; }
    
    [Display(Name = "Descripción de Propiedad")]
    public string? PropiedadDescripcion { get; set; }
    
    [Display(Name = "Cliente")]
    public string ClienteId { get; set; } = string.Empty;
    
    [Display(Name = "Nombre del Cliente")]
    public string ClienteNombre { get; set; } = string.Empty;
    
    [Display(Name = "Monto Ofertado")]
    [DisplayFormat(DataFormatString = "{0:C0}")]
    public decimal MontoOferta { get; set; }
    
    [Display(Name = "Estado")]
    public int Estado { get; set; }
    
    [Display(Name = "Estado")]
    public string EstadoTexto { get; set; } = string.Empty;
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; }
    
    [Display(Name = "Comentarios")]
    public string? Comentarios { get; set; }
    
    
    public bool EsPendiente => Estado == 0; 
    public bool EsAceptada => Estado == 1;  
    public bool EsRechazada => Estado == 2; 
    
    public string MontoFormateado => MontoOferta.ToString("C0");
    public string FechaFormateada => FechaCreacion.ToString("dd/MM/yyyy HH:mm");
    
    public string EstadoClaseCss => Estado switch
    {
        0 => "warning", 
        1 => "success", 
        2 => "danger",  
        _ => "secondary"
    };
}
