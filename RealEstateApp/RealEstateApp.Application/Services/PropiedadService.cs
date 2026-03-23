using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Propiedades;
using Microsoft.Extensions.Logging;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.Application.Services;

public class PropiedadService : IPropiedadService
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PropiedadService> _logger;

    private const decimal PRECIO_MINIMO = 5_000m; 
    private const decimal PRECIO_MAXIMO = 5_000_000_000m; 
    private const int DESCRIPCION_MINIMA_LONGITUD = 10;
    
    private static readonly string[] TiposSinHabitaciones = { "Terreno", "Local Comercial", "Local", "Oficina", "Bodega", "Parqueo" };

    public PropiedadService(
        IPropiedadRepository propiedadRepository,
        ITipoPropiedadRepository tipoPropiedadRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<PropiedadService> logger)
    {
        _propiedadRepository = propiedadRepository;
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<PropiedadViewModel>> GetAllDisponiblesAsync()
    {
        try
        {
            var propiedades = await _propiedadRepository.GetAllDisponiblesAsync();
            var propiedadesViewModel = _mapper.Map<List<PropiedadViewModel>>(propiedades);
            
            return propiedadesViewModel
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedades disponibles");
            return new List<PropiedadViewModel>();
        }
    }

    public async Task<PropiedadViewModel?> GetByCodigoAsync(string codigo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            var propiedad = await _propiedadRepository.GetByCodigoAsync(codigo);
            return propiedad != null ? _mapper.Map<PropiedadViewModel>(propiedad) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedad por código {Codigo}", codigo);
            return null;
        }
    }

    public async Task<PropiedadViewModel?> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0) return null;

            var propiedad = await _propiedadRepository.GetByIdAsync(id);
            return propiedad != null ? _mapper.Map<PropiedadViewModel>(propiedad) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedad por ID {PropiedadId}", id);
            return null;
        }
    }

    public async Task<List<PropiedadViewModel>> GetByFiltrosAsync(FiltrosPropiedadesViewModel filtros)
    {
        try
        {
            if (filtros == null)
                return new List<PropiedadViewModel>();

            if (!filtros.EsRangoPrecioValido())
            {
                throw new ArgumentException("El precio mínimo no puede ser mayor que el precio máximo");
            }

            var propiedades = await _propiedadRepository.GetByFiltrosAsync(
                filtros.TipoPropiedadId,
                filtros.PrecioMinimo,
                filtros.PrecioMaximo,
                filtros.CantidadHabitaciones,
                filtros.CantidadBanos
            );

            var propiedadesViewModel = _mapper.Map<List<PropiedadViewModel>>(propiedades);
            
            return filtros.Descendente
                ? propiedadesViewModel.OrderByDescending(p => p.FechaCreacion).ToList()
                : propiedadesViewModel.OrderBy(p => p.FechaCreacion).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedades por filtros");
            return new List<PropiedadViewModel>();
        }
    }

    public async Task<PropiedadDetalleViewModel?> GetDetalleByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
                return null;

            var propiedad = await _propiedadRepository.GetDetalleByIdAsync(id);
            if (propiedad == null)
                return null;

            var viewModel = _mapper.Map<PropiedadDetalleViewModel>(propiedad);

            if (!string.IsNullOrEmpty(propiedad.AgenteId))
            {
                var agentePerfil = await _usuarioRepository.GetAgentePerfilAsync(propiedad.AgenteId);
                if (agentePerfil != default)
                {
                    viewModel.Agente = new AgenteInfoViewModel
                    {
                        Id = agentePerfil.Id,
                        Nombre = agentePerfil.Nombre,
                        Apellido = agentePerfil.Apellido,
                        Email = agentePerfil.Email,
                        Telefono = agentePerfil.Telefono ?? string.Empty,
                        Foto = agentePerfil.UrlImagen
                    };
                }
                else
                {
                    _logger.LogWarning("No se pudo obtener información del agente {AgenteId} para la propiedad {PropiedadId}", 
                        propiedad.AgenteId, id);
                    
                    viewModel.Agente = new AgenteInfoViewModel
                    {
                        Id = propiedad.AgenteId,
                        Nombre = "Agente",
                        Apellido = "No disponible",
                        Email = "",
                        Telefono = ""
                    };
                }
            }

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de propiedad {PropiedadId}", id);
            return null;
        }
    }

    public async Task<List<PropiedadViewModel>> GetByAgenteIdAsync(string agenteId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(agenteId))
                return new List<PropiedadViewModel>();

            var propiedades = await _propiedadRepository.GetByAgenteIdAsync(agenteId, soloDisponibles: true);
            var propiedadesViewModel = _mapper.Map<List<PropiedadViewModel>>(propiedades);
            
            return propiedadesViewModel
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedades del agente {AgenteId}", agenteId);
            return new List<PropiedadViewModel>();
        }
    }

    public async Task<List<PropiedadViewModel>> GetPropiedadesByAgenteAsync(string agenteId, bool incluirVendidas = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(agenteId))
                return new List<PropiedadViewModel>();

            var propiedades = await _propiedadRepository.GetByAgenteIdAsync(agenteId, soloDisponibles: !incluirVendidas);
            var propiedadesViewModel = _mapper.Map<List<PropiedadViewModel>>(propiedades);
            
            return propiedadesViewModel
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener propiedades del agente {AgenteId}", agenteId);
            return new List<PropiedadViewModel>();
        }
    }

    public async Task<PropiedadViewModel> CreateAsync(SavePropiedadViewModel viewModel, string agenteId)
    {
        try
        {
            await ValidateCreatePropiedadInputAsync(viewModel, agenteId);
            
            var codigo = await _propiedadRepository.GenerarCodigoAsync();
            
            if (await ExisteCodigoAsync(codigo))
                throw new InvalidOperationException("Error generando código único");

            var propiedad = _mapper.Map<Propiedad>(viewModel);
            propiedad.AgenteId = agenteId;
            propiedad.Codigo = codigo; 
            propiedad.Estado = Domain.Enums.EstadoPropiedad.Disponible;
            
            var propiedadCreada = await _propiedadRepository.AddAsync(propiedad);
            
            if (viewModel.MejorasSeleccionadas != null && viewModel.MejorasSeleccionadas.Any())
            {
                foreach (var mejoraId in viewModel.MejorasSeleccionadas)
                {
                    var propiedadMejora = new Domain.Entities.PropiedadMejora
                    {
                        PropiedadId = propiedadCreada.Id,
                        MejoraId = mejoraId
                    };
                    
                    await _propiedadRepository.AddPropiedadMejoraAsync(propiedadMejora);
                }
                
                _logger.LogInformation("Se agregaron {Cantidad} mejoras a la propiedad {PropiedadId}",
                    viewModel.MejorasSeleccionadas.Count, propiedadCreada.Id);
            }
            
            _logger.LogInformation("Propiedad creada: {PropiedadId} - {Codigo} por agente {AgenteId}", 
                propiedadCreada.Id, propiedadCreada.Codigo, agenteId);
            
            var propiedadConRelaciones = await _propiedadRepository.GetByIdAsync(propiedadCreada.Id);
            return _mapper.Map<PropiedadViewModel>(propiedadConRelaciones ?? propiedadCreada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear propiedad para agente {AgenteId}", agenteId);
            throw;
        }
    }

    private async Task ValidateCreatePropiedadInputAsync(SavePropiedadViewModel viewModel, string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            throw new ArgumentException("El ID del agente es requerido", nameof(agenteId));

        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        if (viewModel.TipoPropiedadId <= 0)
            throw new ArgumentException("Debe seleccionar un tipo de propiedad válido", nameof(viewModel.TipoPropiedadId));

        if (viewModel.TipoVentaId <= 0)
            throw new ArgumentException("Debe seleccionar un tipo de venta válido", nameof(viewModel.TipoVentaId));

        if (viewModel.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor que cero", nameof(viewModel.Precio));

        if (viewModel.Precio < PRECIO_MINIMO || viewModel.Precio > PRECIO_MAXIMO)
            throw new ArgumentException($"El precio debe estar en el rango de DOP ${PRECIO_MINIMO:N0} a DOP ${PRECIO_MAXIMO:N0}", nameof(viewModel.Precio));

        var tipoPropiedad = await _tipoPropiedadRepository.GetByIdAsync(viewModel.TipoPropiedadId);
        bool esTipoSimplificado = tipoPropiedad != null && 
            TiposSinHabitaciones.Any(t => tipoPropiedad.Nombre.Contains(t, StringComparison.OrdinalIgnoreCase));

        if (!esTipoSimplificado && viewModel.CantidadHabitaciones <= 0)
            throw new ArgumentException("La cantidad de habitaciones debe ser mayor que cero para este tipo de propiedad", nameof(viewModel.CantidadHabitaciones));

        if (!esTipoSimplificado && viewModel.CantidadBanos <= 0)
            throw new ArgumentException("La cantidad de baños debe ser mayor que cero para este tipo de propiedad", nameof(viewModel.CantidadBanos));

        if (viewModel.TamanoEnMetros <= 0)
            throw new ArgumentException("El tamaño en metros debe ser mayor que cero", nameof(viewModel.TamanoEnMetros));

        if (string.IsNullOrWhiteSpace(viewModel.Descripcion))
            throw new ArgumentException("La descripción es requerida", nameof(viewModel.Descripcion));

        if (viewModel.Descripcion.Trim().Length < DESCRIPCION_MINIMA_LONGITUD)
            throw new ArgumentException($"La descripción debe tener al menos {DESCRIPCION_MINIMA_LONGITUD} caracteres", nameof(viewModel.Descripcion));

        if (!esTipoSimplificado && (viewModel.MejorasSeleccionadas == null || !viewModel.MejorasSeleccionadas.Any()))
            throw new ArgumentException("Debe seleccionar al menos una mejora", nameof(viewModel.MejorasSeleccionadas));
    }

    public async Task<PropiedadViewModel?> UpdateAsync(SavePropiedadViewModel viewModel, string agenteId)
    {
        try
        {
            var propiedad = await _propiedadRepository.GetByIdAsync(viewModel.Id);
            if (propiedad == null || propiedad.AgenteId != agenteId)
                return null;

            await ValidateUpdatePropiedadInputAsync(viewModel, agenteId);
            
            _mapper.Map(viewModel, propiedad);

            await _propiedadRepository.UpdateAsync(propiedad);
            
            if (viewModel.MejorasSeleccionadas != null)
            {
                await _propiedadRepository.RemovePropiedadMejorasAsync(viewModel.Id);
                
                foreach (var mejoraId in viewModel.MejorasSeleccionadas)
                {
                    var propiedadMejora = new Domain.Entities.PropiedadMejora
                    {
                        PropiedadId = viewModel.Id,
                        MejoraId = mejoraId
                    };
                    
                    await _propiedadRepository.AddPropiedadMejoraAsync(propiedadMejora);
                }
                
                _logger.LogInformation("Se actualizaron mejoras para la propiedad {PropiedadId}: {Cantidad} mejoras",
                    viewModel.Id, viewModel.MejorasSeleccionadas.Count);
            }
            
            _logger.LogInformation("Propiedad actualizada: {PropiedadId} - {Codigo}", propiedad.Id, propiedad.Codigo);
            
            var propiedadConRelaciones = await _propiedadRepository.GetByIdAsync(viewModel.Id);
            return _mapper.Map<PropiedadViewModel>(propiedadConRelaciones ?? propiedad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar propiedad {PropiedadId}", viewModel.Id);
            throw;
        }
    }

    private async Task ValidateUpdatePropiedadInputAsync(SavePropiedadViewModel viewModel, string agenteId)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        if (viewModel.Id <= 0)
            throw new ArgumentException("ID de propiedad inválido", nameof(viewModel.Id));

        await ValidateCreatePropiedadInputAsync(viewModel, agenteId);
    }

    public async Task<bool> DeleteAsync(int id, string agenteId)
    {
        try
        {
            var propiedad = await _propiedadRepository.GetByIdAsync(id);
            if (propiedad == null || propiedad.AgenteId != agenteId)
                return false;

            await _propiedadRepository.DeleteAsync(propiedad);
            
            _logger.LogInformation("Propiedad eliminada: {PropiedadId} - {Codigo}", id, propiedad.Codigo);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar propiedad {PropiedadId}", id);
            throw;
        }
    }

    public async Task<bool> ExisteAsync(int id)
    {
        try
        {
            if (id <= 0)
                return false;

            var propiedad = await _propiedadRepository.GetByIdAsync(id);
            return propiedad != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de propiedad {PropiedadId}", id);
            return false;
        }
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            return await _propiedadRepository.ExisteCodigoAsync(codigo, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia del código {Codigo}", codigo);
            return false;
        }
    }

    public async Task<bool> EstaDisponibleAsync(int id)
    {
        try
        {
            if (id <= 0)
                return false;

            return await _propiedadRepository.EstaDisponibleAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar disponibilidad de propiedad {PropiedadId}", id);
            return false;
        }
    }

    public async Task<bool> PerteneceAAgenteAsync(int id, string agenteId)
    {
        try
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(agenteId))
                return false;

            var propiedad = await _propiedadRepository.GetByIdAsync(id);
            return propiedad?.AgenteId == agenteId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar propiedad {PropiedadId} del agente {AgenteId}", id, agenteId);
            return false;
        }
    }

    public async Task<PropiedadesEstadisticasViewModel> GetEstadisticasPropiedadesAsync()
    {
        try
        {
            var estadisticas = await _propiedadRepository.GetEstadisticasAsync();
            return new PropiedadesEstadisticasViewModel
            {
                Disponibles = estadisticas.Disponibles,
                Vendidas = estadisticas.Vendidas
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de propiedades");
            return new PropiedadesEstadisticasViewModel();
        }
    }

    public async Task<int> GetCantidadPropiedadesByAgenteAsync(string agenteId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(agenteId))
                return 0;

            return await _propiedadRepository.GetCantidadByAgenteAsync(agenteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cantidad de propiedades del agente {AgenteId}", agenteId);
            return 0;
        }
    }

    public async Task DeleteAllByAgenteAsync(string agenteId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(agenteId))
                return;

            await _propiedadRepository.DeleteAllByAgenteAsync(agenteId);
            
            _logger.LogWarning("Todas las propiedades del agente {AgenteId} han sido eliminadas", agenteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar todas las propiedades del agente {AgenteId}", agenteId);
            throw;
        }
    }
}
