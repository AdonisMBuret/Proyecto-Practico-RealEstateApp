using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Agentes;

namespace RealEstateApp.WebApp.Controllers;


public class AgentesController : Controller
{
    private readonly ILogger<AgentesController> _logger;
    private readonly IAgenteService _agenteService;
    private readonly IPropiedadService _propiedadService;

    public AgentesController(
        ILogger<AgentesController> logger,
        IAgenteService agenteService,
        IPropiedadService propiedadService)
    {
        _logger = logger;
        _agenteService = agenteService;
        _propiedadService = propiedadService;
    }

    public async Task<IActionResult> Index(string? nombre)
    {
        
            List<AgenteViewModel> agentes;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
               
                agentes = await _agenteService.GetByNombreAsync(nombre.Trim());
            }
            else
            {
               
                agentes = await _agenteService.GetAllActivosAsync();
            }

            var viewModel = new ListadoAgentesViewModel
            {
                Agentes = agentes.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre).ToList(), // ? ORDENAMIENTO ALFABÉTICO
                NombreBusqueda = nombre
            };

            return View(viewModel);
        
    }

   
    public async Task<IActionResult> Propiedades(string id)
    {
        
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "ID de agente no válido";
                return RedirectToAction(nameof(Index));
            }

             var agentePerfil = await _agenteService.GetByIdAsync(id);

            if (agentePerfil == null)
            {
                TempData["Error"] = "Agente no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var esActivo = await _agenteService.EsActivoAsync(id);
            
            if (!esActivo)
            {
                TempData["Warning"] = "Este agente no está activo actualmente";
                return RedirectToAction(nameof(Index));
            }

         
            var propiedades = await _propiedadService.GetByAgenteIdAsync(id);
            
           
            var propiedadesDisponibles = propiedades.Where(p => p.EstadoTexto == "Disponible").ToList();

           
            var agenteViewModel = new AgenteViewModel
            {
                Id = agentePerfil.Id,
                Nombre = agentePerfil.Nombre,
                Apellido = agentePerfil.Apellido,
                NombreCompleto = agentePerfil.NombreCompleto,
                Email = agentePerfil.Email,
                Telefono = agentePerfil.Telefono,
                UrlImagenPerfil = agentePerfil.Foto,
                CantidadPropiedades = propiedadesDisponibles.Count,
                EsActivo = esActivo
            };

            var viewModel = new AgentePropiedadesViewModel
            {
                Agente = agenteViewModel, 
                Propiedades = propiedadesDisponibles 
            };

            return View(viewModel);
       
    }
}
