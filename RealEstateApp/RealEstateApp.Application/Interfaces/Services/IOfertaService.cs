using RealEstateApp.Application.ViewModels.Ofertas;

namespace RealEstateApp.Application.Interfaces.Services;


public interface IOfertaService
{
   
    Task<OfertaViewModel> CrearOfertaAsync(SaveOfertaViewModel oferta);
    Task<List<OfertaViewModel>> GetOfertasByClienteAsync(string clienteId);
    Task<List<OfertaViewModel>> GetOfertasByAgenteAsync(string agenteId);
    Task<List<OfertaViewModel>> GetOfertasByPropiedadAsync(int propiedadId);
    Task<List<OfertaViewModel>> GetOfertasByClienteAndPropiedadAsync(string clienteId, int propiedadId);
    
   
    Task AceptarOfertaAsync(int ofertaId, string agenteId);
    Task RechazarOfertaAsync(int ofertaId, string agenteId, string? comentarios = null);
    
    
    Task<bool> PuedeHacerOfertaAsync(string clienteId, int propiedadId);
    Task<bool> TieneOfertasPendientesAsync(string clienteId, int propiedadId);
    Task<bool> TieneOfertasAceptadasAsync(string clienteId, int propiedadId);
}