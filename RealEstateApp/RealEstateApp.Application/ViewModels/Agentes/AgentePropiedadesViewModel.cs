using System.ComponentModel.DataAnnotations;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Application.ViewModels.Agentes;


public class AgentePropiedadesViewModel
{
    [Display(Name = "Agente")]
    public AgenteViewModel Agente { get; set; } = new();
    
    [Display(Name = "Propiedades")]
    public List<PropiedadViewModel> Propiedades { get; set; } = new();
    

    public bool TienePropiedades => Propiedades.Any();
    public int TotalPropiedades => Propiedades.Count;
}
