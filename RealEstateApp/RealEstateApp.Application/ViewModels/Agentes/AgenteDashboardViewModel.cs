using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Application.ViewModels.Ofertas;

namespace RealEstateApp.Application.ViewModels.Agentes;


public class AgenteDashboardViewModel
{
    public AgentePerfilViewModel Agente { get; set; } = new();
    public int TotalPropiedades { get; set; }
    public int PropiedadesDisponibles { get; set; }
    public int PropiedadesVendidas { get; set; }
    
 
    public List<PropiedadViewModel> Propiedades { get; set; } = new();
    
    public List<OfertaViewModel> OfertasAceptadas { get; set; } = new();
    public decimal TotalGanancias => OfertasAceptadas.Sum(o => o.MontoOferta);
    
    public decimal TotalVentasEnDOP => Propiedades.Where(p => p.EstadoTexto == "Vendida").Sum(p => p.Precio);
    
    public bool TieneVentas => PropiedadesVendidas > 0;
    

    public List<PropiedadViewModel> PropiedadesRecientes => 
        Propiedades.OrderByDescending(p => p.FechaCreacion).Take(3).ToList();

    public double PorcentajeVentas => 
        TotalPropiedades > 0 ? (double)PropiedadesVendidas / TotalPropiedades * 100 : 0;
}
