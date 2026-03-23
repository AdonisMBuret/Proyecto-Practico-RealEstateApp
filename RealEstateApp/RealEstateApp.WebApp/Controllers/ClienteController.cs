using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Identity.Entities;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;


[Authorize(Roles = "Cliente")]
public class ClienteController : Controller
{
    private readonly ILogger<ClienteController> _logger;
    private readonly IFavoritoService _favoritoService;
    private readonly IPropiedadService _propiedadService;
    private readonly IChatService _chatService;
    private readonly IOfertaService _ofertaService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClienteController(
        ILogger<ClienteController> logger,
        IFavoritoService favoritoService,
        IPropiedadService propiedadService,
        IChatService chatService,
        IOfertaService ofertaService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _favoritoService = favoritoService;
        _propiedadService = propiedadService;
        _chatService = chatService;
        _ofertaService = ofertaService;
        _userManager = userManager;
    }

    
    public async Task<IActionResult> MisPropiedades()
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "No se pudo identificar al cliente";
                return RedirectToAction("Index", "Home");
            }

            var propiedadesFavoritas = await _favoritoService.GetPropiedadesFavoritasAsync(clienteId);

            return View(propiedadesFavoritas);
        
    }

   
    public async Task<IActionResult> PropiedadesAdquiridas()
    {
        var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(clienteId))
        {
            TempData["Error"] = "No se pudo identificar al cliente";
            return RedirectToAction("Index", "Home");
        }

        var ofertas = await _ofertaService.GetOfertasByClienteAsync(clienteId);
        var ofertasAceptadas = ofertas.Where(o => o.EstadoTexto == "Aceptada").ToList();

        var propiedadesAdquiridas = new List<PropiedadDetalleViewModel>();
        
        foreach (var oferta in ofertasAceptadas)
        {
            var propiedad = await _propiedadService.GetDetalleByIdAsync(oferta.PropiedadId);
            if (propiedad != null)
            {
                propiedadesAdquiridas.Add(propiedad);
            }
        }

        ViewBag.ClienteId = clienteId;
        ViewBag.CantidadPropiedades = propiedadesAdquiridas.Count;
        ViewBag.OfertasAceptadas = ofertasAceptadas;

        _logger.LogInformation("Cliente {ClienteId} tiene {Cantidad} propiedades adquiridas", 
            clienteId, propiedadesAdquiridas.Count);

        return View(propiedadesAdquiridas);
    }

   
    [HttpPost]
    public async Task<IActionResult> AgregarFavorito(int propiedadId)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
                return Json(new { success = false, message = "Usuario no autenticado" });

            await _favoritoService.AgregarFavoritoAsync(clienteId, propiedadId);

            return Json(new { success = true, message = "Propiedad agregada a favoritos" });
        
    }

    
    [HttpPost]
    public async Task<IActionResult> EliminarFavorito(int propiedadId)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
                return Json(new { success = false, message = "Usuario no autenticado" });

            await _favoritoService.RemoverFavoritoAsync(clienteId, propiedadId);

            return Json(new { success = true, message = "Propiedad removida de favoritos" });
        
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarOferta(int propiedadId, decimal monto)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "No se pudo identificar al cliente";
                return RedirectToAction("Details", "Home", new { id = propiedadId });
            }

            if (monto <= 0)
            {
                TempData["Error"] = "El monto de la oferta debe ser mayor a cero";
                return RedirectToAction("Details", "Home", new { id = propiedadId });
            }

            
            var puedeHacerOferta = await _ofertaService.PuedeHacerOfertaAsync(clienteId, propiedadId);
            
            if (!puedeHacerOferta)
            {
                TempData["Warning"] = "No puede enviar una oferta en este momento. Ya tiene una oferta pendiente o aceptada para esta propiedad.";
                return RedirectToAction("Details", "Home", new { id = propiedadId });
            }

            var saveOferta = new SaveOfertaViewModel
            {
                ClienteId = clienteId,
                PropiedadId = propiedadId,
                MontoOferta = monto 
            };

            var resultado = await _ofertaService.CrearOfertaAsync(saveOferta); 

            if (resultado != null)
            {
                TempData["Success"] = "Oferta enviada exitosamente. El agente será notificado.";
            }
            else
            {
                TempData["Error"] = "No se pudo enviar la oferta";
            }

            return RedirectToAction("Details", "Home", new { id = propiedadId });
        
    }

    
    public async Task<IActionResult> MisOfertas(int? propiedadId)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "No se pudo identificar al cliente";
                return RedirectToAction("Index", "Home");
            }

            List<OfertaViewModel> ofertas;

            if (propiedadId.HasValue)
            {
                ofertas = await _ofertaService.GetOfertasByClienteAndPropiedadAsync(clienteId, propiedadId.Value);
            }
            else
            {
               
                ofertas = await _ofertaService.GetOfertasByClienteAsync(clienteId);
            }

            ViewBag.PropiedadId = propiedadId;
            return View(ofertas.OrderByDescending(o => o.FechaCreacion).ToList());
        
    }

    
    public async Task<IActionResult> Chat(int propiedadId)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "No se pudo identificar al cliente";
                return RedirectToAction("Index", "Home");
            }

            var propiedadDetalle = await _propiedadService.GetDetalleByIdAsync(propiedadId);
            
            if (propiedadDetalle == null)
            {
                TempData["Error"] = "La propiedad no fue encontrada";
                return RedirectToAction("Index", "Home");
            }

            var mensajes = await _chatService.GetMensajesByConversacionAsync(propiedadId, clienteId, propiedadDetalle.Agente.Id);

            ViewBag.PropiedadId = propiedadId;
            ViewBag.AgenteId = propiedadDetalle.Agente.Id;
            ViewBag.AgenteNombre = propiedadDetalle.Agente.Nombre;
            ViewBag.CodigoPropiedad = propiedadDetalle.Codigo;
            ViewBag.TipoPropiedad = propiedadDetalle.TipoPropiedad;

            return View(mensajes);
        
    }

 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnviarMensajeChat(int propiedadId, string agenteId, string mensaje)
    {
        
            var clienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(clienteId))
            {
                TempData["Error"] = "No se pudo identificar al cliente";
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                TempData["Error"] = "El mensaje no puede estar vacío";
                return RedirectToAction("Chat", new { propiedadId });
            }

            var mensajeViewModel = new SaveMensajeViewModel
            {
                PropiedadId = propiedadId,
                EmisorId = clienteId,
                ReceptorId = agenteId,
                Contenido = mensaje.Trim()
            };

            var resultado = await _chatService.EnviarMensajeAsync(mensajeViewModel);

            if (resultado != null)
            {
                TempData["Success"] = "Mensaje enviado exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo enviar el mensaje";
            }

            return RedirectToAction("Chat", new { propiedadId });
        
    }

    
    [HttpPost]
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
}
