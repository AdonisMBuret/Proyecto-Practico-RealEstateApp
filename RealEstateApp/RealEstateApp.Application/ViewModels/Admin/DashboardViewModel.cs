namespace RealEstateApp.Application.ViewModels.Admin;

public class DashboardViewModel
{

    public int PropiedadesDisponibles { get; set; }
    public int PropiedadesVendidas { get; set; }
    public int TotalPropiedades => PropiedadesDisponibles + PropiedadesVendidas;
    public decimal PorcentajeVendidas => TotalPropiedades > 0 
        ? (decimal)PropiedadesVendidas / TotalPropiedades * 100 
        : 0;

    
    public int AgentesActivos { get; set; }
    public int AgentesInactivos { get; set; }
    public int TotalAgentes => AgentesActivos + AgentesInactivos;

   
    public int ClientesActivos { get; set; }
    public int ClientesInactivos { get; set; }
    public int TotalClientes => ClientesActivos + ClientesInactivos;

 
    public int DesarrolladoresActivos { get; set; }
    public int DesarrolladoresInactivos { get; set; }
    public int TotalDesarrolladores => DesarrolladoresActivos + DesarrolladoresInactivos;

    
    public int TotalTipoPropiedades { get; set; }
    public int TotalTipoVentas { get; set; }
    public int TotalMejoras { get; set; }
    public int TotalOfertas { get; set; }
}
