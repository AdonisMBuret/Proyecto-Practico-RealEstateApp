namespace RealEstateApp.Application.ViewModels.Ofertas;


public class OfertasClienteViewModel
{
    public string ClienteId { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public int CantidadOfertas { get; set; }
    public OfertaViewModel UltimaOferta { get; set; } = new();
    

    public bool TieneOfertasPendientes => UltimaOferta.EstadoTexto == "Pendiente";
}