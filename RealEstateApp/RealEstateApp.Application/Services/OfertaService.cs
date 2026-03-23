using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;

public class OfertaService : IOfertaService
{
    private readonly IOfertaRepository _ofertaRepository;
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<OfertaService> _logger;

    
    private const decimal MONTO_MINIMO_OFERTA = 1_000m; 
    public OfertaService(
        IOfertaRepository ofertaRepository,
        IPropiedadRepository propiedadRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<OfertaService> logger)
    {
        _ofertaRepository = ofertaRepository;
        _propiedadRepository = propiedadRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OfertaViewModel> CrearOfertaAsync(SaveOfertaViewModel oferta)
    {
        if (oferta == null)
            throw new ArgumentNullException(nameof(oferta));

        
        if (oferta.MontoOferta <= 0)
            throw new ArgumentException("El monto de la oferta debe ser mayor que cero", nameof(oferta.MontoOferta));

        if (oferta.MontoOferta < MONTO_MINIMO_OFERTA)
            throw new ArgumentException($"El monto de la oferta debe ser al menos {MONTO_MINIMO_OFERTA:N0} DOP", nameof(oferta.MontoOferta));

        if (string.IsNullOrWhiteSpace(oferta.ClienteId))
            throw new ArgumentException("El ID del cliente es requerido", nameof(oferta.ClienteId));

        if (oferta.PropiedadId <= 0)
            throw new ArgumentException("El ID de la propiedad debe ser mayor que cero", nameof(oferta.PropiedadId));

        
        var clienteExiste = await _usuarioRepository.GetByIdAsync(oferta.ClienteId);
        if (!clienteExiste)
            throw new InvalidOperationException("Cliente no encontrado");

       
        var propiedadDisponible = await _propiedadRepository.EstaDisponibleAsync(oferta.PropiedadId);
        if (!propiedadDisponible)
            throw new InvalidOperationException("La propiedad no existe o no está disponible");

      
        var puedeHacer = await PuedeHacerOfertaAsync(oferta.ClienteId, oferta.PropiedadId);
        if (!puedeHacer)
            throw new InvalidOperationException("No puede hacer una oferta en esta propiedad en este momento");

       
        var nuevaOferta = new Oferta
        {
            PropiedadId = oferta.PropiedadId,
            ClienteId = oferta.ClienteId,
            Monto = oferta.MontoOferta,
            Comentarios = oferta.Comentarios,
            Estado = EstadoOferta.Pendiente,
            FechaCreacion = DateTime.Now
        };

        var ofertaCreada = await _ofertaRepository.AddAsync(nuevaOferta);
        
        var resultado = _mapper.Map<OfertaViewModel>(ofertaCreada);
        
        _logger.LogInformation("Oferta creada: {OfertaId} por cliente {ClienteId} en propiedad {PropiedadId}", 
            ofertaCreada.Id, oferta.ClienteId, oferta.PropiedadId);
        
        return resultado;
    }

    public async Task<List<OfertaViewModel>> GetOfertasByClienteAsync(string clienteId)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            return new List<OfertaViewModel>();

        var ofertas = await _ofertaRepository.GetByClienteAsync(clienteId);
        var ofertasViewModel = _mapper.Map<List<OfertaViewModel>>(ofertas);
        
        return ofertasViewModel
            .OrderByDescending(o => o.FechaCreacion)
            .ToList();
    }

    public async Task<List<OfertaViewModel>> GetOfertasByAgenteAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return new List<OfertaViewModel>();

        var ofertas = await _ofertaRepository.GetByAgenteAsync(agenteId);
        var ofertasViewModel = _mapper.Map<List<OfertaViewModel>>(ofertas);
        
        foreach (var ofertaVm in ofertasViewModel)
        {
            var ofertaOriginal = ofertas.FirstOrDefault(o => o.Id == ofertaVm.Id);
            if (ofertaOriginal != null)
            {
                try
                {
                    var clientePerfil = await _usuarioRepository.GetUsuarioPerfilAsync(ofertaOriginal.ClienteId);
                    if (clientePerfil != default)
                    {
                        ofertaVm.ClienteNombre = $"{clientePerfil.Nombre} {clientePerfil.Apellido}";
                    }
                    else
                    {
                        ofertaVm.ClienteNombre = "Cliente no disponible";
                    }
                }
                catch
                {
                    ofertaVm.ClienteNombre = "Cliente no disponible";
                }
            }
        }
        
        return ofertasViewModel
            .OrderByDescending(o => o.FechaCreacion)
            .ToList();
    }

    public async Task<List<OfertaViewModel>> GetOfertasByPropiedadAsync(int propiedadId)
    {
        if (propiedadId <= 0)
            return new List<OfertaViewModel>();

        var ofertas = await _ofertaRepository.GetByPropiedadAsync(propiedadId);
        var ofertasViewModel = _mapper.Map<List<OfertaViewModel>>(ofertas);
        
        foreach (var ofertaVm in ofertasViewModel)
        {
            var ofertaOriginal = ofertas.FirstOrDefault(o => o.Id == ofertaVm.Id);
            if (ofertaOriginal != null)
            {
                try
                {
                    var clientePerfil = await _usuarioRepository.GetUsuarioPerfilAsync(ofertaOriginal.ClienteId);
                    if (clientePerfil != default)
                    {
                        ofertaVm.ClienteNombre = $"{clientePerfil.Nombre} {clientePerfil.Apellido}";
                    }
                    else
                    {
                        ofertaVm.ClienteNombre = "Cliente no disponible";
                    }
                }
                catch
                {
                    ofertaVm.ClienteNombre = "Cliente no disponible";
                }
            }
        }
        
        return ofertasViewModel
            .OrderByDescending(o => o.FechaCreacion)
            .ToList();
    }

    public async Task<List<OfertaViewModel>> GetOfertasByClienteAndPropiedadAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId) || propiedadId <= 0)
            return new List<OfertaViewModel>();

        var ofertas = await _ofertaRepository.GetByClienteAndPropiedadAsync(clienteId, propiedadId);
        var ofertasViewModel = _mapper.Map<List<OfertaViewModel>>(ofertas);
        
        foreach (var ofertaVm in ofertasViewModel)
        {
            var ofertaOriginal = ofertas.FirstOrDefault(o => o.Id == ofertaVm.Id);
            if (ofertaOriginal != null)
            {
                try
                {
                    var clientePerfil = await _usuarioRepository.GetUsuarioPerfilAsync(ofertaOriginal.ClienteId);
                    if (clientePerfil != default)
                    {
                        ofertaVm.ClienteNombre = $"{clientePerfil.Nombre} {clientePerfil.Apellido}";
                    }
                    else
                    {
                        ofertaVm.ClienteNombre = "Cliente no disponible";
                    }
                }
                catch
                {
                    ofertaVm.ClienteNombre = "Cliente no disponible";
                }
            }
        }
        
        return ofertasViewModel
            .OrderByDescending(o => o.FechaCreacion)
            .ToList();
    }

    public async Task AceptarOfertaAsync(int ofertaId, string agenteId)
    {
        if (ofertaId <= 0)
            throw new ArgumentException("El ID de la oferta debe ser mayor que cero", nameof(ofertaId));

        if (string.IsNullOrWhiteSpace(agenteId))
            throw new ArgumentException("El ID del agente no puede estar vacío", nameof(agenteId));

        var oferta = await _ofertaRepository.GetByIdAsync(ofertaId);
        if (oferta == null)
            throw new InvalidOperationException("Oferta no encontrada");

        var propiedad = await _propiedadRepository.GetByIdAsync(oferta.PropiedadId);
        if (propiedad == null || propiedad.AgenteId != agenteId)
            throw new InvalidOperationException("No tiene permisos para modificar esta oferta");

        if (oferta.Estado != EstadoOferta.Pendiente)
            throw new InvalidOperationException("Solo se pueden aceptar ofertas pendientes");

        var tieneOfertaAceptada = await _ofertaRepository.HasAcceptedOfertaAsync(oferta.PropiedadId);
        if (tieneOfertaAceptada)
            throw new InvalidOperationException("Esta propiedad ya tiene una oferta aceptada");

        oferta.Estado = EstadoOferta.Aceptada;
        await _ofertaRepository.UpdateAsync(oferta);

        propiedad.Estado = EstadoPropiedad.Vendida;
        await _propiedadRepository.UpdateAsync(propiedad);

        var todasLasOfertas = await _ofertaRepository.GetByPropiedadAsync(oferta.PropiedadId);
        var ofertasPendientes = todasLasOfertas.Where(o => o.Id != ofertaId && o.Estado == EstadoOferta.Pendiente).ToList();
        
        foreach (var ofertaPendiente in ofertasPendientes)
        {
            ofertaPendiente.Estado = EstadoOferta.Rechazada;
            await _ofertaRepository.UpdateAsync(ofertaPendiente);
        }

        _logger.LogInformation(
            "Oferta {OfertaId} aceptada por agente {AgenteId}. Propiedad {PropiedadId} marcada como vendida. {CantidadRechazadas} ofertas rechazadas automáticamente",
            ofertaId, agenteId, oferta.PropiedadId, ofertasPendientes.Count);
    }

    public async Task RechazarOfertaAsync(int ofertaId, string agenteId, string? comentarios = null)
    {
        if (ofertaId <= 0)
            throw new ArgumentException("El ID de la oferta debe ser mayor que cero", nameof(ofertaId));

        if (string.IsNullOrWhiteSpace(agenteId))
            throw new ArgumentException("El ID del agente no puede estar vacío", nameof(agenteId));

        var oferta = await _ofertaRepository.GetByIdAsync(ofertaId);
        if (oferta == null)
            throw new InvalidOperationException("Oferta no encontrada");

        var propiedad = await _propiedadRepository.GetByIdAsync(oferta.PropiedadId);
        if (propiedad == null || propiedad.AgenteId != agenteId)
            throw new InvalidOperationException("No tiene permisos para modificar esta oferta");

        if (oferta.Estado != EstadoOferta.Pendiente)
            throw new InvalidOperationException("Solo se pueden rechazar ofertas pendientes");

        oferta.Estado = EstadoOferta.Rechazada;

        await _ofertaRepository.UpdateAsync(oferta);
        
        _logger.LogInformation("Oferta {OfertaId} rechazada por agente {AgenteId}", ofertaId, agenteId);
    }

    public async Task<bool> PuedeHacerOfertaAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId) || propiedadId <= 0)
            return false;

        try
        {
            var disponible = await _propiedadRepository.EstaDisponibleAsync(propiedadId);
            if (!disponible)
                return false;

            var tieneAceptadas = await TieneOfertasAceptadasAsync(clienteId, propiedadId);
            if (tieneAceptadas)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si puede hacer oferta cliente {ClienteId} en propiedad {PropiedadId}", clienteId, propiedadId);
            return false;
        }
    }

    public async Task<bool> TieneOfertasPendientesAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId) || propiedadId <= 0)
            return false;

        try
        {
            return await _ofertaRepository.TieneOfertasPendientesAsync(clienteId, propiedadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar ofertas pendientes para cliente {ClienteId} en propiedad {PropiedadId}", clienteId, propiedadId);
            return false;
        }
    }

    public async Task<bool> TieneOfertasAceptadasAsync(string clienteId, int propiedadId)
    {
        if (string.IsNullOrWhiteSpace(clienteId) || propiedadId <= 0)
            return false;

        try
        {
            return await _ofertaRepository.TieneOfertasAceptadasAsync(clienteId, propiedadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar ofertas aceptadas para cliente {ClienteId} en propiedad {PropiedadId}", clienteId, propiedadId);
            return false;
        }
    }
}
