namespace RealEstateApp.Application.ViewModels.Ofertas;

public class OfertasPorPropiedadViewModel
{
    public int PropiedadId { get; set; }
    public string PropiedadCodigo { get; set; } = string.Empty;
    public string PropiedadDescripcion { get; set; } = string.Empty;
    public int CantidadOfertas { get; set; }
    public int OfertasPendientes { get; set; }
    public decimal MontoMaximo { get; set; }
    public DateTime UltimaOfertaFecha { get; set; }
    
    public bool TieneOfertasPendientes => OfertasPendientes > 0;
}
