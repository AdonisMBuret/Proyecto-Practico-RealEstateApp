using RealEstateApp.Domain.Entities;

namespace RealEstateApp.Domain.Interfaces;

public interface IOfertaRepository : IRepositoryAsync<Oferta>
{
    Task<List<Oferta>> GetByClienteAsync(string clienteId);
    Task<List<Oferta>> GetByAgenteAsync(string agenteId);
    Task<List<Oferta>> GetByPropiedadAsync(int propiedadId);
    Task<List<Oferta>> GetByClienteAndPropiedadAsync(string clienteId, int propiedadId);
    Task<bool> TieneOfertasPendientesAsync(string clienteId, int propiedadId);
    Task<bool> TieneOfertasAceptadasAsync(string clienteId, int propiedadId);
    Task<int> GetCantidadOfertasByAgenteAsync(string agenteId);
    Task<bool> HasAcceptedOfertaAsync(int propiedadId);
}
