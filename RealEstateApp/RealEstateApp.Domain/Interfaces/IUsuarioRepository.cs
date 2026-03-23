namespace RealEstateApp.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<List<string>> GetAgenteActivosIdsAsync();
    Task<bool> UpdateAgenteAsync(string agenteId, string nombre, string apellido, string telefono, string? urlImagen);
    Task<bool> ExisteAgenteAsync(string id);
    Task<bool> IsAgenteActivoAsync(string id);
    Task<int> GetCantidadPropiedadesByAgenteAsync(string agenteId);
    Task<List<string>> GetAgentesByNombreIdsAsync(string nombre);
    Task<(string Id, string Nombre, string Apellido, string Email, string? Telefono, string? UrlImagen)> GetAgentePerfilAsync(string agenteId);
    Task<(string Id, string Nombre, string Apellido, string Email, string? Telefono, string? UrlImagen)> GetUsuarioPerfilAsync(string usuarioId);
    Task<bool> GetByIdAsync(string id);
    Task<bool> EsAgenteActivoAsync(string id);
    
    Task<object?> GetUsuarioByIdAsync(string id);
}
