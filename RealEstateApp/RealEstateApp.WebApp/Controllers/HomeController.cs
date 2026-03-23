using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Application.ViewModels.Chat;
using System.Security.Claims;
using System.Diagnostics;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Application.ViewModels.Common;

namespace RealEstateApp.WebApp.Controllers;
public class HomeController : Controller 
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPropiedadService _propiedadService;
    private readonly ITipoPropiedadService _tipoPropiedadService;
    private readonly IFavoritoService _favoritoService;
    private readonly IChatService _chatService;
    private readonly IOfertaService _ofertaService;
    //ya yere, estas ahí??
    public HomeController(
        ILogger<HomeController> logger,
        IPropiedadService propiedadService,
        ITipoPropiedadService tipoPropiedadService,
        IFavoritoService favoritoService,
        IChatService chatService,
        IOfertaService ofertaService)
    {
        _logger = logger;
        _propiedadService = propiedadService;
        _tipoPropiedadService = tipoPropiedadService;
        _favoritoService = favoritoService;
        _chatService = chatService;
        _ofertaService = ofertaService;
    }

    
    public async Task<IActionResult> Index(FiltrosPropiedadesViewModel? filtros)
    {
        List<PropiedadViewModel> propiedades;

        if (filtros != null && !string.IsNullOrWhiteSpace(filtros.CodigoPropiedad))
        {
            var propiedad = await _propiedadService.GetByCodigoAsync(filtros.CodigoPropiedad);
            propiedades = propiedad != null ? new List<PropiedadViewModel> { propiedad } : new List<PropiedadViewModel>();
        }
        else if (filtros != null && TieneFiltrosAplicados(filtros))
        {
            propiedades = await _propiedadService.GetByFiltrosAsync(filtros);
        }
        else
        {
            propiedades = await _propiedadService.GetAllDisponiblesAsync();
        }

        var tiposPropiedades = await _tipoPropiedadService.GetAllAsync();

        var viewModel = new HomeViewModel
        {
            Propiedades = propiedades,
            Filtros = filtros ?? new FiltrosPropiedadesViewModel()
        };

        if (User.IsInRole("Cliente"))
        {
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!string.IsNullOrEmpty(clienteId))
            {
                viewModel.ClienteId = clienteId;
                var favoritasIds = await _favoritoService.GetPropiedadesFavoritasIdsAsync(clienteId);
                viewModel.PropiedadesFavoritas = favoritasIds;
            }
        }

        ViewBag.TiposPropiedades = tiposPropiedades;

        return View(viewModel);
       
    }


    public async Task<IActionResult> Details(int id)
    {
        
            var propiedadDetalle = await _propiedadService.GetDetalleByIdAsync(id);

            if (propiedadDetalle == null)
            {
                TempData["Error"] = "La propiedad solicitada no fue encontrada";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new DetallePropiedadViewModel
            {
                Propiedad = propiedadDetalle,
                PuedeEnviarMensajes = User.Identity?.IsAuthenticated ?? false,
                PuedeHacerOfertas = User.IsInRole("Cliente")
            };

           
            if (User.IsInRole("Cliente"))
            {
                var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (!string.IsNullOrEmpty(clienteId))
                {
                  
                    viewModel.EsFavorita = await _favoritoService.EsFavoritoAsync(clienteId, id);
                    
                 
                    viewModel.Ofertas = await _ofertaService.GetOfertasByClienteAndPropiedadAsync(clienteId, id);
                    
                    
                    viewModel.TieneOfertaAceptada = viewModel.Ofertas.Any(o => o.EstadoTexto == "Aceptada");
                    viewModel.TieneOfertaPendiente = viewModel.Ofertas.Any(o => o.EstadoTexto == "Pendiente");
                    
                   
                    viewModel.Mensajes = await _chatService.GetMensajesByConversacionAsync(id, clienteId, propiedadDetalle.Agente.Id);
                }
            }

            return View(viewModel);
        
    }


    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MisFavoritos()
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "No se pudo identificar al cliente";
                return RedirectToAction(nameof(Index));
            }

            var propiedadesFavoritas = await _favoritoService.GetPropiedadesFavoritasAsync(clienteId);

            var viewModel = new HomeViewModel
            {
                Propiedades = propiedadesFavoritas,
                ClienteId = clienteId,
                PropiedadesFavoritas = propiedadesFavoritas.Select(p => p.Id).ToList()
            };

            return View("Index", viewModel); 
       
    }

  
    [HttpPost]
    [Authorize(Roles = "Cliente")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarMensaje(int propiedadId, string mensaje)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                TempData["Error"] = "El mensaje no puede estar vacío";
                return RedirectToAction("Details", new { id = propiedadId });
            }

            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var propiedadDetalle = await _propiedadService.GetDetalleByIdAsync(propiedadId);

            if (propiedadDetalle == null)
            {
                TempData["Error"] = "Propiedad no encontrada";
                return RedirectToAction(nameof(Index));
            }

            // ? Validar que el agente tenga ID
            if (string.IsNullOrWhiteSpace(propiedadDetalle.Agente?.Id))
            {
                _logger.LogError("El agente de la propiedad {PropiedadId} no tiene ID asignado", propiedadId);
                TempData["Error"] = "No se pudo identificar al agente de esta propiedad";
                return RedirectToAction("Details", new { id = propiedadId });
            }

            var mensajeViewModel = new SaveMensajeViewModel
            {
                PropiedadId = propiedadId,
                EmisorId = clienteId!,
                ReceptorId = propiedadDetalle.Agente.Id,
                Contenido = mensaje.Trim()
            };

            var resultado = await _chatService.EnviarMensajeAsync(mensajeViewModel);

            if (resultado != null)
            {
                TempData["Success"] = "Mensaje enviado exitosamente al agente";
            }
            else
            {
                TempData["Error"] = "No se pudo enviar el mensaje";
            }

            return RedirectToAction("Details", new { id = propiedadId });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al enviar mensaje en propiedad {PropiedadId}", propiedadId);
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", new { id = propiedadId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al enviar mensaje en propiedad {PropiedadId}", propiedadId);
            TempData["Error"] = "Ocurrió un error al enviar el mensaje. Por favor intente nuevamente.";
            return RedirectToAction("Details", new { id = propiedadId });
        }
    }

  
    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> ToggleFavorito(int propiedadId)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
                return Json(new { success = false, message = "Usuario no autenticado" });

            var esFavorito = await _favoritoService.EsFavoritoAsync(clienteId, propiedadId);
            
            if (esFavorito)
            {
                await _favoritoService.RemoverFavoritoAsync(clienteId, propiedadId);
                return Json(new { success = true, isFavorite = false, message = "Removido de favoritos" });
            }
            else
            {
                await _favoritoService.AgregarFavoritoAsync(clienteId, propiedadId);
                return Json(new { success = true, isFavorite = true, message = "Agregado a favoritos" });
            }
       
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HacerOferta(int propiedadId, decimal montoOferta, string? comentarios)
    {
        try
        {
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "Usuario no autenticado";
                return RedirectToAction("Details", new { id = propiedadId });
            }

            var propiedad = await _propiedadService.GetDetalleByIdAsync(propiedadId);
            if (propiedad == null)
            {
                TempData["Error"] = "Propiedad no encontrada";
                return RedirectToAction(nameof(Index));
            }

            if (montoOferta <= 0)
            {
                TempData["Error"] = "El monto de la oferta debe ser mayor a cero";
                return RedirectToAction("Details", new { id = propiedadId });
            }

           
            var montoMinimo = propiedad.Precio * 0.10m; 
            if (montoOferta < montoMinimo)
            {
                TempData["Error"] = $"El monto de la oferta debe ser al menos el 10% del precio de la propiedad: {montoMinimo:C0} DOP";
                return RedirectToAction("Details", new { id = propiedadId });
            }

            var ofertasExistentes = await _ofertaService.GetOfertasByClienteAndPropiedadAsync(clienteId, propiedadId);
            var tieneOfertaPendiente = ofertasExistentes.Any(e => e.EstadoTexto == "Pendiente");
            
            if (tieneOfertaPendiente)
            {
                TempData["Warning"] = "Ya tienes una oferta pendiente para esta propiedad";
                return RedirectToAction("Details", new { id = propiedadId });
            }

            var todasLasOfertas = await _ofertaService.GetOfertasByPropiedadAsync(propiedadId);
            var tieneOfertaAceptada = todasLasOfertas.Any(o => o.EstadoTexto == "Aceptada");
            
            if (tieneOfertaAceptada)
            {
                TempData["Warning"] = "Esta propiedad ya tiene una oferta aceptada";
                return RedirectToAction("Details", new { id = propiedadId });
            }

            var ofertaViewModel = new SaveOfertaViewModel
            {
                PropiedadId = propiedadId,
                ClienteId = clienteId,
                MontoOferta = montoOferta,
                Comentarios = comentarios?.Trim()
            };

            var resultado = await _ofertaService.CrearOfertaAsync(ofertaViewModel);
            
            if (resultado != null)
            {
                if (!string.IsNullOrWhiteSpace(propiedad.Agente?.Id))
                {
                    try
                    {
                        var mensajeOferta = $"Nueva oferta recibida por {montoOferta:C0} DOP";
                        
                        if (!string.IsNullOrWhiteSpace(comentarios))
                        {
                            mensajeOferta += $"\n\nComentarios del cliente:\n{comentarios}";
                        }

                        var mensajeViewModel = new SaveMensajeViewModel
                        {
                            PropiedadId = propiedadId,
                            EmisorId = clienteId,
                            ReceptorId = propiedad.Agente.Id,
                            Contenido = mensajeOferta
                        };

                        await _chatService.EnviarMensajeAsync(mensajeViewModel);
                        
                        _logger.LogInformation(
                            "Mensaje automático enviado al agente {AgenteId} por oferta de cliente {ClienteId} en propiedad {PropiedadId}",
                            propiedad.Agente.Id, clienteId, propiedadId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, 
                            "No se pudo enviar el mensaje automático al agente, pero la oferta fue creada exitosamente");
                    }
                }

                TempData["Success"] = $"Oferta de {montoOferta:C0} enviada exitosamente al agente";
                _logger.LogInformation("Oferta creada: Cliente {ClienteId}, Propiedad {PropiedadId}, Monto {Monto}", 
                    clienteId, propiedadId, montoOferta);
            }
            else
            {
                TempData["Error"] = "No se pudo enviar la oferta. Intente nuevamente";
            }

            return RedirectToAction("Details", new { id = propiedadId });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al hacer oferta en propiedad {PropiedadId}", propiedadId);
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", new { id = propiedadId });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Operación inválida al hacer oferta en propiedad {PropiedadId}", propiedadId);
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", new { id = propiedadId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al hacer oferta en propiedad {PropiedadId}", propiedadId);
            TempData["Error"] = "Ocurrió un error al enviar la oferta. Por favor intente nuevamente.";
            return RedirectToAction("Details", new { id = propiedadId });
        }
    }


    private static bool TieneFiltrosAplicados(FiltrosPropiedadesViewModel filtros)
    {
        return filtros.TipoPropiedadId.HasValue ||
               filtros.PrecioMinimo.HasValue ||
               filtros.PrecioMaximo.HasValue ||
               filtros.CantidadHabitaciones.HasValue ||
               filtros.CantidadBanos.HasValue;
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new RealEstateApp.Application.ViewModels.Common.ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}
