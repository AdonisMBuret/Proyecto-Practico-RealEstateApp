using AutoMapper;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;


public class AgenteService : IAgenteService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IMapper _mapper;

    public AgenteService(
        IUsuarioRepository usuarioRepository,
        IPropiedadRepository propiedadRepository,
        IMapper mapper)
    {
        _usuarioRepository = usuarioRepository;
        _propiedadRepository = propiedadRepository;
        _mapper = mapper;
    }

    public async Task<List<AgenteViewModel>> GetAllActivosAsync()
    {
        var agentesIds = await _usuarioRepository.GetAgenteActivosIdsAsync();
        var agentesViewModel = new List<AgenteViewModel>();

        foreach (var agenteId in agentesIds)
        {
            var (Id, Nombre, Apellido, Email, Telefono, UrlImagen) = await _usuarioRepository.GetAgentePerfilAsync(agenteId);

            if (!string.IsNullOrEmpty(Id))
            {
                var cantidadPropiedades = await _propiedadRepository.GetCantidadByAgenteAsync(agenteId);

                agentesViewModel.Add(new AgenteViewModel
                {
                    Id = Id,
                    Nombre = Nombre,
                    Apellido = Apellido,
                    NombreCompleto = $"{Nombre} {Apellido}",
                    Email = Email,
                    Telefono = Telefono,
                    UrlImagenPerfil = UrlImagen,
                    EsActivo = true,
                    CantidadPropiedades = cantidadPropiedades
                });
            }
        }

        return agentesViewModel.OrderBy(a => a.NombreCompleto).ToList();
    }

    public async Task<AgentePerfilViewModel?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var existeAgente = await _usuarioRepository.ExisteAgenteAsync(id);
        if (!existeAgente)
            return null;

        var (Id, Nombre, Apellido, Email, Telefono, UrlImagen) = await _usuarioRepository.GetAgentePerfilAsync(id);

        if (string.IsNullOrEmpty(Id))
            return null;

        return new AgentePerfilViewModel
        {
            Id = Id,
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            Telefono = Telefono,
            Foto = UrlImagen
        };
    }

    public async Task<List<AgenteViewModel>> GetByNombreAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return new List<AgenteViewModel>();

        var agentesIds = await _usuarioRepository.GetAgentesByNombreIdsAsync(nombre);
        var agentesViewModel = new List<AgenteViewModel>();

        foreach (var agenteId in agentesIds)
        {
            var (Id, Nombre, Apellido, Email, Telefono, UrlImagen) = await _usuarioRepository.GetAgentePerfilAsync(agenteId);

            if (!string.IsNullOrEmpty(Id))
            {
                var cantidadPropiedades = await _propiedadRepository.GetCantidadByAgenteAsync(agenteId);

                agentesViewModel.Add(new AgenteViewModel
                {
                    Id = Id,
                    Nombre = Nombre,
                    Apellido = Apellido,
                    NombreCompleto = $"{Nombre} {Apellido}",
                    Email = Email,
                    Telefono = Telefono,
                    UrlImagenPerfil = UrlImagen,
                    EsActivo = true,
                    CantidadPropiedades = cantidadPropiedades
                });
            }
        }

        return agentesViewModel.OrderBy(a => a.NombreCompleto).ToList();
    }

    public async Task<AgentePerfilViewModel?> GetPerfilAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return null;

        var (Id, Nombre, Apellido, Email, Telefono, UrlImagen) = await _usuarioRepository.GetAgentePerfilAsync(agenteId);

        if (string.IsNullOrEmpty(Id))
            return null;

        return new AgentePerfilViewModel
        {
            Id = Id,
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            Telefono = Telefono,
            Foto = UrlImagen
        };
    }

    public async Task<bool> ActualizarPerfilAsync(string agenteId, EditarAgenteViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return false;

        if (viewModel == null)
            return false;

        var existeAgente = await _usuarioRepository.ExisteAgenteAsync(agenteId);
        if (!existeAgente)
            return false;

        return await _usuarioRepository.UpdateAgenteAsync(
            agenteId,
            viewModel.Nombre,
            viewModel.Apellido,
            viewModel.Telefono,
            viewModel.FotoActual);
    }

    public async Task<bool> ExisteAgenteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return await _usuarioRepository.ExisteAgenteAsync(id);
    }

    public async Task<bool> EsActivoAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return await _usuarioRepository.EsAgenteActivoAsync(id);
    }

    public async Task<int> GetCantidadPropiedadesAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return 0;

        return await _propiedadRepository.GetCantidadByAgenteAsync(agenteId);
    }
}