using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Application.ViewModels.Admin;


public class AdminDashboardViewModel
{
   
    public int PropiedadesDisponibles { get; set; }
    public int PropiedadesVendidas { get; set; }
    public int TotalPropiedades => PropiedadesDisponibles + PropiedadesVendidas;
    public double PorcentajePropiedadesVendidas => TotalPropiedades > 0 ? (double)PropiedadesVendidas / TotalPropiedades * 100 : 0;


    public int AgentesActivos { get; set; }
    public int AgentesInactivos { get; set; }
    public int TotalAgentes => AgentesActivos + AgentesInactivos;

   
    public int ClientesActivos { get; set; }
    public int ClientesInactivos { get; set; }
    public int TotalClientes => ClientesActivos + ClientesInactivos;

 
    public int DesarrolladoresActivos { get; set; }
    public int DesarrolladoresInactivos { get; set; }
    public int TotalDesarrolladores => DesarrolladoresActivos + DesarrolladoresInactivos;
}
