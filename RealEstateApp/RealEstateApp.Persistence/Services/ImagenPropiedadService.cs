using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Services;

public class ImagenPropiedadService : IImagenPropiedadService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ImagenPropiedadService> _logger;

    public ImagenPropiedadService(ApplicationDbContext context, ILogger<ImagenPropiedadService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddImagenAsync(int propiedadId, string urlImagen, bool esPrincipal = false)
    {
        try
        {
            var imagen = new ImagenPropiedad
            {
                PropiedadId = propiedadId,
                UrlImagen = urlImagen,
                EsPrincipal = esPrincipal
            };

            _context.ImagenesPropiedades.Add(imagen);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Imagen agregada para propiedad {PropiedadId}: {UrlImagen}", propiedadId, urlImagen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar imagen para propiedad {PropiedadId}", propiedadId);
            throw;
        }
    }

    public async Task<List<string>> GetImagenesByPropiedadIdAsync(int propiedadId)
    {
        try
        {
            return await _context.ImagenesPropiedades
                .Where(i => i.PropiedadId == propiedadId)
                .Select(i => i.UrlImagen)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener imágenes de propiedad {PropiedadId}", propiedadId);
            return new List<string>();
        }
    }

    public async Task DeleteImagenByUrlAsync(int propiedadId, string urlImagen)
    {
        try
        {
            var imagen = await _context.ImagenesPropiedades
                .FirstOrDefaultAsync(i => i.PropiedadId == propiedadId && i.UrlImagen == urlImagen);

            if (imagen != null)
            {
                _context.ImagenesPropiedades.Remove(imagen);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Imagen eliminada de propiedad {PropiedadId}: {UrlImagen}", propiedadId, urlImagen);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar imagen de propiedad {PropiedadId}", propiedadId);
            throw;
        }
    }

    public async Task DeleteAllImagenesByPropiedadIdAsync(int propiedadId)
    {
        try
        {
            var imagenes = await _context.ImagenesPropiedades
                .Where(i => i.PropiedadId == propiedadId)
                .ToListAsync();

            if (imagenes.Any())
            {
                _context.ImagenesPropiedades.RemoveRange(imagenes);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Se eliminaron {Cantidad} imágenes de propiedad {PropiedadId}", imagenes.Count, propiedadId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar todas las imágenes de propiedad {PropiedadId}", propiedadId);
            throw;
        }
    }

    public async Task SetImagenPrincipalAsync(int propiedadId, string urlImagen)
    {
        try
        {
            // Quitar el flag de principal de todas las imágenes de la propiedad
            var imagenes = await _context.ImagenesPropiedades
                .Where(i => i.PropiedadId == propiedadId)
                .ToListAsync();

            foreach (var img in imagenes)
            {
                img.EsPrincipal = img.UrlImagen == urlImagen;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Imagen principal establecida para propiedad {PropiedadId}: {UrlImagen}", propiedadId, urlImagen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al establecer imagen principal para propiedad {PropiedadId}", propiedadId);
            throw;
        }
    }
}
