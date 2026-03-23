namespace RealEstateApp.Application.DTOs.Agentes;

public class AgenteDTO
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public int CantidadPropiedades { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}
