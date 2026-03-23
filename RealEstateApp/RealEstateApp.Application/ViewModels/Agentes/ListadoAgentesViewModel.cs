using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Agentes;

public class ListadoAgentesViewModel
{
    public List<AgenteViewModel> Agentes { get; set; } = new();
    
    [Display(Name = "Buscar por nombre")]
    public string? NombreBusqueda { get; set; }
}
