using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Application.ViewModels.Ofertas;

namespace RealEstateApp.Application.ViewModels.Propiedades;


public class DetallePropiedadViewModel
{
    public PropiedadDetalleViewModel Propiedad { get; set; } = new();
    
    public bool PuedeEnviarMensajes { get; set; }
    public bool PuedeHacerOfertas { get; set; }
    public bool EsFavorita { get; set; }
    
    
    public bool TieneOfertaAceptada { get; set; }
    public bool TieneOfertaPendiente { get; set; }
    
    public List<OfertaViewModel> Ofertas { get; set; } = new();
    public List<MensajeViewModel> Mensajes { get; set; } = new();
}
