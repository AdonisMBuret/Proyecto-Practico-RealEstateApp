namespace RealEstateApp.Application.DTOs.Agentes;

public class AgenteApiDTO
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public int CantidadPropiedades { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
}