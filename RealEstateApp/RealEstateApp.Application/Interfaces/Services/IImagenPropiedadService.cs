namespace RealEstateApp.Application.Interfaces.Services;


public interface IImagenPropiedadService
{
    Task AddImagenAsync(int propiedadId, string urlImagen, bool esPrincipal = false);
    Task<List<string>> GetImagenesByPropiedadIdAsync(int propiedadId);
    Task DeleteImagenByUrlAsync(int propiedadId, string urlImagen);
    Task DeleteAllImagenesByPropiedadIdAsync(int propiedadId);
    Task SetImagenPrincipalAsync(int propiedadId, string urlImagen);
}
