using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.WebApp.Controllers;


[Authorize(Roles = "Administrador")]
public class TipoPropiedadesController : Controller
{
    private readonly ITipoPropiedadService _tipoPropiedadService;
    private readonly ILogger<TipoPropiedadesController> _logger;

    public TipoPropiedadesController(
        ITipoPropiedadService tipoPropiedadService,
        ILogger<TipoPropiedadesController> logger)
    {
        _tipoPropiedadService = tipoPropiedadService;
        _logger = logger;
    }


    public async Task<IActionResult> Index()
    {
        
            var tipos = await _tipoPropiedadService.GetAllAsync();
            return View(tipos);
    }


    public async Task<IActionResult> Details(int id)
    {
        
            var tipo = await _tipoPropiedadService.GetByIdAsync(id);
            
            if (tipo == null)
            {
                TempData["Error"] = "El tipo de propiedad no fue encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        
    }


    public IActionResult Create()
    {
        return View(new SaveTipoPropiedadViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveTipoPropiedadViewModel viewModel)
    {
        
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _tipoPropiedadService.CreateAsync(viewModel);
            
            TempData["Success"] = $"Tipo de propiedad '{viewModel.Nombre}' creado exitosamente";
            return RedirectToAction(nameof(Index));
        
    }

  
    public async Task<IActionResult> Edit(int id)
    {
        
            var tipo = await _tipoPropiedadService.GetByIdAsync(id);
            
            if (tipo == null)
            {
                TempData["Error"] = "El tipo de propiedad no fue encontrado";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new SaveTipoPropiedadViewModel
            {
                Nombre = tipo.Nombre,
                Descripcion = tipo.Descripcion
            };

            ViewData["TipoPropiedadId"] = id;
            ViewData["TipoPropiedadNombre"] = tipo.Nombre;

            return View(viewModel);
        
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaveTipoPropiedadViewModel viewModel)
    {
        
            if (!ModelState.IsValid)
            {
                ViewData["TipoPropiedadId"] = id;
                return View(viewModel);
            }

            await _tipoPropiedadService.UpdateAsync(id, viewModel);
            
            TempData["Success"] = $"Tipo de propiedad '{viewModel.Nombre}' actualizado exitosamente";
            return RedirectToAction(nameof(Index));
       
    }

    public async Task<IActionResult> Delete(int id)
    {
        
            var tipo = await _tipoPropiedadService.GetByIdAsync(id);
            
            if (tipo == null)
            {
                TempData["Error"] = "El tipo de propiedad no fue encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
       
    }

 
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        
            var tipo = await _tipoPropiedadService.GetByIdAsync(id);
            
            if (tipo == null)
            {
                TempData["Error"] = "El tipo de propiedad no fue encontrado";
                return RedirectToAction(nameof(Index));
            }

            await _tipoPropiedadService.DeleteAsync(id);
            
            TempData["Success"] = $"Tipo de propiedad '{tipo.Nombre}' eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        
    }
}
