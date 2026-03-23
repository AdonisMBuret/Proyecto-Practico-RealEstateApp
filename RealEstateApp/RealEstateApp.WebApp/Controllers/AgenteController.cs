using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.ViewModels.Catalogos;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Application.ViewModels.Propiedades;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;


[Authorize(Roles = "Agente")]
public class AgenteController : Controller
{
    private readonly ILogger<AgenteController> _logger;
    private readonly IAgenteService _agenteService;
    private readonly IPropiedadService _propiedadService;
    private readonly IOfertaService _ofertaService;
    private readonly IChatService _chatService;
    private readonly ITipoPropiedadService _tipoPropiedadService;
    private readonly ITipoVentaService _tipoVentaService;
    private readonly IMejoraService _mejoraService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IImagenPropiedadService _imagenPropiedadService; 
    private readonly INotificacionService _notificacionService;

    public AgenteController(
        ILogger<AgenteController> logger,
        IAgenteService agenteService,
        IPropiedadService propiedadService,
        IOfertaService ofertaService,
        IChatService chatService,
        ITipoPropiedadService tipoPropiedadService,
        ITipoVentaService tipoVentaService,
        IMejoraService mejoraService,
        IFileUploadService fileUploadService,
        IImagenPropiedadService imagenPropiedadService,
        INotificacionService notificacionService) 
    {
        _logger = logger;
        _agenteService = agenteService;
        _propiedadService = propiedadService;
        _ofertaService = ofertaService;
        _chatService = chatService;
        _tipoPropiedadService = tipoPropiedadService;
        _tipoVentaService = tipoVentaService;
        _mejoraService = mejoraService;
        _fileUploadService = fileUploadService;
        _imagenPropiedadService = imagenPropiedadService; // NUEVO adoni
        _notificacionService = notificacionService;
    }


    public async Task<IActionResult> Index()
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(agenteId))
            {
                TempData["Error"] = "No se pudo identificar al agente";
                return RedirectToAction("Index", "Home");
            }

            var agente = await _agenteService.GetByIdAsync(agenteId);
            if (agente == null)
            {
                TempData["Error"] = "Agente no encontrado";
                return RedirectToAction("Index", "Home");
            }

           
            var todasLasPropiedades = await _propiedadService.GetPropiedadesByAgenteAsync(agenteId, incluirVendidas: true);

            var todasLasOfertas = await _ofertaService.GetOfertasByAgenteAsync(agenteId);
            var ofertasAceptadas = todasLasOfertas.Where(o => o.EstadoTexto == "Aceptada").ToList();

            var viewModel = new AgenteDashboardViewModel
            {
                Agente = agente, 
                TotalPropiedades = todasLasPropiedades.Count,
                PropiedadesDisponibles = todasLasPropiedades.Count(p => p.EstadoTexto == "Disponible"),
                PropiedadesVendidas = todasLasPropiedades.Count(p => p.EstadoTexto == "Vendida"),
                Propiedades = todasLasPropiedades.OrderByDescending(p => p.FechaCreacion).Take(10).ToList(), 
                OfertasAceptadas = ofertasAceptadas
            };

            _logger.LogInformation("Dashboard cargado para agente {AgenteId}: {Total} propiedades total, {Disponibles} disponibles, {Vendidas} vendidas", 
                agenteId, viewModel.TotalPropiedades, viewModel.PropiedadesDisponibles, viewModel.PropiedadesVendidas);

            return View(viewModel);
        
    }

    
    public async Task<IActionResult> MiPerfil()
    {
    try
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var agente = await _agenteService.GetByIdAsync(agenteId!);

        if (agente == null)
        {
            TempData["Error"] = "Agente no encontrado";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new EditarAgenteViewModel
        {
            Nombre = agente.Nombre,
            Apellido = agente.Apellido,
            Telefono = agente.Telefono,
            Email = agente.Email,
            FotoActual = agente.Foto ?? string.Empty 

        };

        return View(viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al cargar perfil del agente");
        TempData["Error"] = "Ocurrió unerror al cargar el perfil";
        return RedirectToAction(nameof(Index));
    }
}

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MiPerfil(EditarAgenteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
           
            if (model.NuevaFoto != null && _fileUploadService.IsValidImage(model.NuevaFoto))
            {
                var rutaImagen = await _fileUploadService.UploadImageAsync(model.NuevaFoto, "usuarios");
                model.FotoActual = rutaImagen; 
            }
            
            var resultado = await _agenteService.ActualizarPerfilAsync(agenteId!, model);

            if (resultado)
            {
                TempData["Success"] = "Perfil actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "No se pudo actualizar el perfil";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar perfil del agente");
            TempData["Error"] = "Ocurrió un error al actualizar el perfil";
            return View(model);
        }
    }

   
    public async Task<IActionResult> MantenimientoPropiedades()
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            
            var propiedadesDisponibles = await _propiedadService.GetPropiedadesByAgenteAsync(agenteId!, incluirVendidas: false);

            return View(propiedadesDisponibles);
        
    }

  
    public async Task<IActionResult> CrearPropiedad()
    {
        
           
            var tiposPropiedades = await _tipoPropiedadService.GetAllAsync();
            var tiposVentas = await _tipoVentaService.GetAllAsync();
            var mejoras = await _mejoraService.GetAllAsync();

            if (!tiposPropiedades.Any() || !tiposVentas.Any() || !mejoras.Any())
            {
                var mensajeError = "No se puede crear propiedades. Faltan: ";
                var faltantes = new List<string>();

                if (!tiposPropiedades.Any()) faltantes.Add("Tipos de Propiedades");
                if (!tiposVentas.Any()) faltantes.Add("Tipos de Ventas");
                if (!mejoras.Any()) faltantes.Add("Mejoras");

                TempData["Error"] = mensajeError + string.Join(", ", faltantes);
                return RedirectToAction(nameof(MantenimientoPropiedades));
            }

            ViewBag.TiposPropiedades = tiposPropiedades;
            ViewBag.TiposVentas = tiposVentas;
            ViewBag.Mejoras = mejoras;

            return View(new SavePropiedadViewModel());
        
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPropiedad(SavePropiedadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CargarCatalogosParaFormulario();
            return View(model);
        }

        try
        {
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var resultado = await _propiedadService.CreateAsync(model, agenteId!);

            if (resultado != null)
            {
                //cambio nuevo adoni
                if (model.Imagenes != null && model.Imagenes.Any())
                {
                    var imagenesGuardadas = new List<string>();
                    bool esPrimeraImagen = true;

                    foreach (var imagen in model.Imagenes)
                    {
                        if (imagen != null && imagen.Length > 0 && _fileUploadService.IsValidImage(imagen))
                        {
                            // Subir imagen al sistema de archivos
                            var rutaImagen = await _fileUploadService.UploadImageAsync(imagen, "propiedades");
                            
                            if (!string.IsNullOrEmpty(rutaImagen))
                            {
                                // Guardar referencia en la base de datos
                                await _imagenPropiedadService.AddImagenAsync(resultado.Id, rutaImagen, esPrimeraImagen);
                                imagenesGuardadas.Add(rutaImagen);
                                esPrimeraImagen = false; 
                            }
                        }
                    }

                    _logger.LogInformation("Se guardaron {Cantidad} imágenes para la propiedad {PropiedadId}", 
                        imagenesGuardadas.Count, resultado.Id);
                }

                TempData["Success"] = $"Propiedad creada exitosamente con código: {resultado.Codigo}";
                return RedirectToAction(nameof(MantenimientoPropiedades));
            }

            TempData["Error"] = "No se pudo crear la propiedad";
            await CargarCatalogosParaFormulario();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear propiedad");
            TempData["Error"] = $"Error al crear la propiedad: {ex.Message}";
            await CargarCatalogosParaFormulario();
            return View(model);
        }
    }

    
    public async Task<IActionResult> EditarPropiedad(int id)
    {
        try
        {
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var propiedad = await _propiedadService.GetByIdAsync(id);

            if (propiedad == null || propiedad.AgenteId != agenteId)
            {
                TempData["Error"] = "Propiedad no encontrada o no tiene permisos para editarla";
                return RedirectToAction(nameof(MantenimientoPropiedades));
            }

            await CargarCatalogosParaFormulario();

            var imagenesActuales = await _imagenPropiedadService.GetImagenesByPropiedadIdAsync(id);
            
          
            var todasLasMejoras = await _mejoraService.GetAllAsync();
            var mejorasIds = todasLasMejoras
                .Where(m => propiedad.Mejoras.Contains(m.Nombre))
                .Select(m => m.Id)
                .ToList();

            var viewModel = new SavePropiedadViewModel
            {
                Id = propiedad.Id,
                TipoPropiedadId = propiedad.TipoPropiedadId,
                TipoVentaId = propiedad.TipoVentaId,
                Precio = propiedad.Precio, 
                Descripcion = propiedad.Descripcion,
                TamanoEnMetros = (int)propiedad.TamanoEnMetros,
                CantidadHabitaciones = propiedad.CantidadHabitaciones,
                CantidadBanos = propiedad.CantidadBanos,
                MejorasSeleccionadas = mejorasIds, 
                ImagenesActuales = imagenesActuales 
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el formulario de edición para propiedad {PropiedadId}", id);
            TempData["Error"] = "Ocurrió un error al cargar la propiedad";
            return RedirectToAction(nameof(MantenimientoPropiedades));
        }
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPropiedad(int id, SavePropiedadViewModel model)
    {
        ModelState.Remove("Imagenes");
        ModelState.Remove("ImagenesActuales");
        ModelState.Remove("ImagenesAEliminar");
        
        if (!ModelState.IsValid)
        {
            var imagenesActuales = await _imagenPropiedadService.GetImagenesByPropiedadIdAsync(id);
            model.ImagenesActuales = imagenesActuales;
            
            await CargarCatalogosParaFormulario();
            return View(model);
        }

        try
        {
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.Id = id;
            
            var resultado = await _propiedadService.UpdateAsync(model, agenteId!);

            if (resultado != null)
            {
                if (model.ImagenesAEliminar != null && model.ImagenesAEliminar.Any())
                {
                    foreach (var imagenUrl in model.ImagenesAEliminar)
                    {
                        await _imagenPropiedadService.DeleteImagenByUrlAsync(id, imagenUrl);
                        await _fileUploadService.DeleteImageAsync(imagenUrl);
                    }
                    
                    _logger.LogInformation("Se eliminaron {Cantidad} imágenes de la propiedad {PropiedadId}",
                        model.ImagenesAEliminar.Count, id);
                }

                if (model.Imagenes != null && model.Imagenes.Any())
                {
                    var imagenesExistentes = await _imagenPropiedadService.GetImagenesByPropiedadIdAsync(id);
                    bool esPrimeraImagen = !imagenesExistentes.Any();

                    foreach (var imagen in model.Imagenes)
                    {
                        if (imagen != null && imagen.Length > 0 && _fileUploadService.IsValidImage(imagen))
                        {
                            var rutaImagen = await _fileUploadService.UploadImageAsync(imagen, "propiedades");
                            
                            if (!string.IsNullOrEmpty(rutaImagen))
                            {
                                await _imagenPropiedadService.AddImagenAsync(id, rutaImagen, esPrimeraImagen);
                                esPrimeraImagen = false;
                            }
                        }
                    }
                    
                    _logger.LogInformation("Se agregaron {Cantidad} nuevas imágenes a la propiedad {PropiedadId}",
                        model.Imagenes.Count, id);
                }

                TempData["Success"] = "Propiedad actualizada exitosamente";
                return RedirectToAction(nameof(MantenimientoPropiedades));
            }

            TempData["Error"] = "No se pudo actualizar la propiedad";
            await CargarCatalogosParaFormulario();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar propiedad {PropiedadId}", id);
            TempData["Error"] = $"Error al actualizar la propiedad: {ex.Message}";
            await CargarCatalogosParaFormulario();
            return View(model);
        }
    }

    
    public async Task<IActionResult> EliminarPropiedad(int id)
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var propiedad = await _propiedadService.GetByIdAsync(id);

            if (propiedad == null || propiedad.AgenteId != agenteId)
            {
                TempData["Error"] = "Propiedad no encontrada o no tiene permisos";
                return RedirectToAction(nameof(MantenimientoPropiedades));
            }

            return View(propiedad);
       
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarPropiedadConfirmado(int id)
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var resultado = await _propiedadService.DeleteAsync(id, agenteId!);

            if (resultado)
            {
                TempData["Success"] = "Propiedad eliminada exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la propiedad";
            }

            return RedirectToAction(nameof(MantenimientoPropiedades));
        
    }

   
    public async Task<IActionResult> Conversaciones()
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(agenteId))
            {
                TempData["Error"] = "No se pudo identificar al agente";
                return RedirectToAction(nameof(Index));
            }

            var conversaciones = await _chatService.GetConversacionesByAgenteAsync(agenteId);

            var conversacionesConPropiedad = new List<ConversacionConPropiedadViewModel>();
            
            foreach (var conversacion in conversaciones)
            {
                var propiedad = await _propiedadService.GetByIdAsync(conversacion.PropiedadId);
                
                conversacionesConPropiedad.Add(new ConversacionConPropiedadViewModel
                {
                    Conversacion = conversacion,
                    PropiedadCodigo = propiedad?.Codigo ?? "N/A",
                    PropiedadDescripcion = propiedad?.Descripcion ?? "Propiedad no disponible"
                });
            }

            ViewBag.MostrandoTodasLasConversaciones = true;
            return View("ConversacionesGeneral", conversacionesConPropiedad);
       
    }

    
    public async Task<IActionResult> Chat(int propiedadId, string clienteId)
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var mensajes = await _chatService.GetMensajesByConversacionAsync(propiedadId, clienteId, agenteId!);

            var conversaciones = await _chatService.GetConversacionesByAgenteAsync(agenteId!);
            var conversacion = conversaciones.FirstOrDefault(c => c.ClienteId == clienteId && c.PropiedadId == propiedadId);
            
            ViewBag.PropiedadId = propiedadId;
            ViewBag.ClienteId = clienteId;
            ViewBag.AgenteId = agenteId;
            ViewBag.ClienteNombre = conversacion?.ClienteNombre ?? "Cliente";

            return View(mensajes);
       
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResponderMensaje(int propiedadId, string clienteId, string mensaje)
    {
        
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                TempData["Error"] = "El mensaje no puede estar vacío";
                return RedirectToAction(nameof(Chat), new { propiedadId, clienteId });
            }

            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var mensajeViewModel = new SaveMensajeViewModel
            {
                PropiedadId = propiedadId,
                EmisorId = agenteId!,
                ReceptorId = clienteId,
                Contenido = mensaje.Trim()
            };

            await _chatService.EnviarMensajeAsync(mensajeViewModel);

            TempData["Success"] = "Mensaje enviado exitosamente";
            return RedirectToAction(nameof(Chat), new { propiedadId, clienteId });
        
    }

    
    public async Task<IActionResult> Ofertas()
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(agenteId))
            {
                TempData["Error"] = "No se pudo identificar al agente";
                return RedirectToAction(nameof(Index));
            }

            var todasLasOfertas = await _ofertaService.GetOfertasByAgenteAsync(agenteId);
            
            var ofertasPorPropiedad = todasLasOfertas
                .GroupBy(o => o.PropiedadId)
                .Select(g => new OfertasPorPropiedadViewModel
                {
                    PropiedadId = g.Key,
                    PropiedadCodigo = g.First().PropiedadCodigo ?? "N/A",
                    PropiedadDescripcion = g.First().PropiedadDescripcion ?? "Sin descripción",
                    CantidadOfertas = g.Count(),
                    OfertasPendientes = g.Count(o => o.EstadoTexto == "Pendiente"),
                    MontoMaximo = g.Max(o => o.MontoOferta),
                    UltimaOfertaFecha = g.Max(o => o.FechaCreacion)
                })
                .OrderByDescending(o => o.OfertasPendientes)
                .ThenByDescending(o => o.UltimaOfertaFecha)
                .ToList();

            return View("OfertasGeneral", ofertasPorPropiedad);
        
    }

    
    public async Task<IActionResult> OfertasCliente(int propiedadId, string clienteId)
    {
        
            var todasOfertas = await _ofertaService.GetOfertasByPropiedadAsync(propiedadId);
            var ofertasCliente = todasOfertas.Where(o => o.ClienteId == clienteId).OrderByDescending(o => o.FechaCreacion).ToList();

            if (!ofertasCliente.Any())
            {
                TempData["Warning"] = "No se encontraron ofertas de este cliente";
                return RedirectToAction(nameof(Ofertas), new { propiedadId });
            }

            ViewBag.PropiedadId = propiedadId;
            ViewBag.ClienteId = clienteId;
            ViewBag.ClienteNombre = ofertasCliente.First().ClienteNombre;

            return View(ofertasCliente);
        
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AceptarOferta(int ofertaId, int propiedadId, string clienteId)
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            await _ofertaService.AceptarOfertaAsync(ofertaId, agenteId!);

            TempData["Success"] = "Oferta aceptada exitosamente. La propiedad ha sido marcada como vendida y las demás ofertas han sido rechazadas automáticamente.";
            return RedirectToAction(nameof(Index));
        
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RechazarOferta(int ofertaId, int propiedadId, string clienteId, string? comentarios = null)
    {
        
            var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            await _ofertaService.RechazarOfertaAsync(ofertaId, agenteId!, comentarios);

            TempData["Success"] = "Oferta rechazada exitosamente";
            return RedirectToAction(nameof(OfertasCliente), new { propiedadId, clienteId });
       
    }

   
    private async Task CargarCatalogosParaFormulario()
    {
        
            ViewBag.TiposPropiedades = await _tipoPropiedadService.GetAllAsync();
            ViewBag.TiposVentas = await _tipoVentaService.GetAllAsync();
            ViewBag.Mejoras = await _mejoraService.GetAllAsync();
       
    }

   
    public async Task<IActionResult> Notificaciones()
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(agenteId))
        {
            TempData["Error"] = "No se pudo identificar al agente";
            return RedirectToAction(nameof(Index));
        }

        var notificaciones = await _notificacionService.GetNotificacionesAgenteAsync(agenteId);
        var resumen = await _notificacionService.GetResumenNotificacionesAgenteAsync(agenteId);

        ViewBag.ResumenNotificaciones = resumen;

        return View(notificaciones);
    }

   
    [HttpGet]
    public async Task<IActionResult> GetNotificaciones()
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(agenteId))
            return Json(new { totalNoLeidas = 0, mensajesNuevos = 0, ofertasNuevas = 0, ultimasNotificaciones = new List<object>() });

        var notificaciones = await _notificacionService.GetResumenNotificacionesAgenteAsync(agenteId);
        
        return Json(notificaciones);
    }

    
    [HttpPost]
    public async Task<IActionResult> MarcarNotificacionLeida(int id)
    {
        await _notificacionService.MarcarComoLeidaAsync(id);
        return Ok();
    }

   
    public async Task<IActionResult> ConversacionesPropiedad(int propiedadId)
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var conversaciones = await _chatService.GetConversacionesByAgenteAsync(agenteId!);
        var conversacionesPropiedad = conversaciones.Where(c => c.PropiedadId == propiedadId).ToList();

        ViewBag.PropiedadId = propiedadId;
        return View("Conversaciones", conversacionesPropiedad);
    }

   
    public async Task<IActionResult> MisPropiedades()
    {
        var agenteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(agenteId))
        {
            TempData["Error"] = "No se pudo identificar al agente";
            return RedirectToAction(nameof(Index));
        }

        var propiedadesDisponibles = await _propiedadService.GetPropiedadesByAgenteAsync(agenteId, incluirVendidas: false);

        return View(propiedadesDisponibles);
    }
    
   
    public async Task<IActionResult> OfertasPropiedad(int propiedadId)
    {
        var ofertas = await _ofertaService.GetOfertasByPropiedadAsync(propiedadId);

        var ofertasPorCliente = ofertas.GroupBy(o => o.ClienteId)
            .Select(g => new OfertasClienteViewModel
            {
                ClienteId = g.Key,
                ClienteNombre = g.First().ClienteNombre,
                CantidadOfertas = g.Count(),
                UltimaOferta = g.OrderByDescending(o => o.FechaCreacion).First()
            })
            .ToList();

        ViewBag.PropiedadId = propiedadId;
        return View("Ofertas", ofertasPorCliente);
    }
}
