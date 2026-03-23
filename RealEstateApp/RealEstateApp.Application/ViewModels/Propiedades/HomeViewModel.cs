namespace RealEstateApp.Application.ViewModels.Propiedades;


public class HomeViewModel
{
    public List<PropiedadViewModel> Propiedades { get; set; } = new();
    public FiltrosPropiedadesViewModel Filtros { get; set; } = new();
    public string? ClienteId { get; set; }
    public List<int> PropiedadesFavoritas { get; set; } = new();
}
