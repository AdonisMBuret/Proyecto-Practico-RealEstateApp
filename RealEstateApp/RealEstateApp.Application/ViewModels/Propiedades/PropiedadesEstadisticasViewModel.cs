namespace RealEstateApp.Application.ViewModels.Propiedades;


public class PropiedadesEstadisticasViewModel
{
    public int Disponibles { get; set; }
    public int Vendidas { get; set; }
    public int Total => Disponibles + Vendidas;
}