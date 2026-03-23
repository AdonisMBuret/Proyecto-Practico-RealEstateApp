using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Agentes;


public class AgentePerfilViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Foto { get; set; }
    
 
    public string NombreCompleto => $"{Nombre} {Apellido}";
    

    public string Iniciales => $"{Nombre.FirstOrDefault()}{Apellido.FirstOrDefault()}".ToUpper();
    
    
    public bool TieneFoto => !string.IsNullOrEmpty(Foto);
}