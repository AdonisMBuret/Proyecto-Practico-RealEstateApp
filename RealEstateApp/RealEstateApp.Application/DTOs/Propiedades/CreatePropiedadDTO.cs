using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.DTOs.Propiedades;

public class CreatePropiedadDTO
{
    [Required]
    public decimal Precio { get; set; }
    
    [Required]
    public double TamanoEnMetros { get; set; }
    
    [Required]
    public int CantidadHabitaciones { get; set; }
    
    [Required]
    public int CantidadBanos { get; set; }
    
    [Required]
    public string Descripcion { get; set; } = string.Empty;
    
    [Required]
    public int TipoPropiedadId { get; set; }
    
    [Required]
    public int TipoVentaId { get; set; }
    
    [Required]
    public string AgenteId { get; set; } = string.Empty;
    
    public List<int> MejorasIds { get; set; } = new();
}