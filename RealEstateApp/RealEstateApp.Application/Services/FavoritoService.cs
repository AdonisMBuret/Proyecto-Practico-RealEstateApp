using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;


public class FavoritoService : IFavoritoService
{
    private readonly IFavoritoRepository _favoritoRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<FavoritoService> _logger;

    public FavoritoService(
        IFavoritoRepository favoritoRepository,
        IPropiedadRepository propiedadRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<FavoritoService> logger)
    {
        _favoritoRepository = favoritoRepository;
        _propiedadRepository = propiedadRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task AgregarFavoritoAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            throw new ArgumentException("El ID del cliente no puede estar vacío", nameof(clienteId));

        if (propiedadId <= 0)
            throw new ArgumentException("El ID de la propiedad debe ser mayor que cero", nameof(propiedadId));

       
        var clienteExiste = await _usuarioRepository.GetByIdAsync(clienteId);
        if (!clienteExiste)
            throw new InvalidOperationException("Cliente no encontrado");

        
        var propiedadDisponible = await _propiedadRepository.EstaDisponibleAsync(propiedadId);
        if (!propiedadDisponible)
            throw new InvalidOperationException("La propiedad no existe o no está disponible");

         var yaEsFavorito = await _favoritoRepository.EsFavoritoAsync(clienteId, propiedadId);
        if (yaEsFavorito)
            throw new InvalidOperationException("La propiedad ya está en favoritos");

        var favorito = new PropiedadFavorita
        {
            ClienteId = clienteId,
            PropiedadId = propiedadId
             };

        await _favoritoRepository.AddAsync(favorito);

        _logger.LogInformation("Propiedad {PropiedadId} agregada a favoritos del cliente {ClienteId}", propiedadId, clienteId);
    }

    public async Task RemoverFavoritoAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            throw new ArgumentException("El ID del cliente no puede estar vacío", nameof(clienteId));

        if (propiedadId <= 0)
            throw new ArgumentException("El ID de la propiedad debe ser mayor que cero", nameof(propiedadId));

        var favorito = await _favoritoRepository.GetByClienteYPropiedadAsync(clienteId, propiedadId);
        if (favorito == null)
            throw new InvalidOperationException("La propiedad no está en favoritos");

        await _favoritoRepository.DeleteAsync(favorito);

        _logger.LogInformation("Propiedad {PropiedadId} removida de favoritos del cliente {ClienteId}", propiedadId, clienteId);
    }

    public async Task<bool> EsFavoritoAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId) || propiedadId <= 0)
            return false;

        try
        {
            return await _favoritoRepository.EsFavoritoAsync(clienteId, propiedadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar favorito para cliente {ClienteId} y propiedad {PropiedadId}", clienteId, propiedadId);
            return false;
        }
    }

    public async Task<List<int>> GetPropiedadesFavoritasIdsAsync(string clienteId)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            return new List<int>();

        try
        {
            return await _favoritoRepository.GetPropiedadesFavoritasIdsAsync(clienteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener IDs de favoritos del cliente {ClienteId}", clienteId);
            return new List<int>();
        }
    }

    public async Task<List<PropiedadViewModel>> GetPropiedadesFavoritasAsync(string clienteId)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            return new List<PropiedadViewModel>();

        try
        {
            var propiedades = await _favoritoRepository.GetPropiedadesFavoritasAsync(clienteId);

       
            var propiedadesViewModel = _mapper.Map<List<PropiedadViewModel>>(propiedades);

           
            return propiedadesViewModel
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedades favoritas del cliente {ClienteId}", clienteId);
            return new List<PropiedadViewModel>();
        }
    }

    public async Task<int> GetCantidadFavoritosAsync(string clienteId)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            return 0;

        try
        {
            return await _favoritoRepository.GetCantidadFavoritosAsync(clienteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cantidad de favoritos del cliente {ClienteId}", clienteId);
            return 0;
        }
    }
}
