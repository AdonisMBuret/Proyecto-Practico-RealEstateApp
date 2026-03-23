namespace RealEstateApp.Application.ViewModels.Admin;


public class UsuarioViewModel
{
    public string Id { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public bool EsActivo { get; set; }
    public string Rol { get; set; } = null!;
}
