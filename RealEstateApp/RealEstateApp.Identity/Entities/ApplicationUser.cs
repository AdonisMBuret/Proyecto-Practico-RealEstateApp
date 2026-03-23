using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Identity.Entities;


public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Cedula { get; set; }
    public string? UrlImagenPerfil { get; set; }
    public bool EsActivo { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public string NombreCompleto => $"{Nombre} {Apellido}";
}
