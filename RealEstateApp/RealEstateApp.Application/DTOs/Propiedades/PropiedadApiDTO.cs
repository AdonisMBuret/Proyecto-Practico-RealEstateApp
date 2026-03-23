namespace RealEstateApp.Application.DTOs.Propiedades;

public class PropiedadApiDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoPropiedad { get; set; } = string.Empty;
    public string TipoVenta { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public decimal TamanoEnMetros { get; set; } 
    public int CantidadHabitaciones { get; set; }
    public int CantidadBanos { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Mejoras { get; set; } = new();
    public string NombreAgente { get; set; } = string.Empty;
    public string IdAgente { get; set; } = string.Empty;
    public string EstadoPropiedad { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}