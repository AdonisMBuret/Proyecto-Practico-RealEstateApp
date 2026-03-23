using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.WebApp.Controllers;


[Authorize(Roles = "Administrador")]
public class MejorasController : Controller
{
    private readonly IMejoraService _mejoraService;
    private readonly ILogger<MejorasController> _logger;

    public MejorasController(
        IMejoraService mejoraService,
        ILogger<MejorasController> logger)
    {
        _mejoraService = mejoraService;
        _logger = logger;
    }

  
    public async Task<IActionResult> Index()
    {
        
            var mejoras = await _mejoraService.GetAllAsync();
            return View(mejoras.OrderBy(m => m.Nombre).ToList()); 
        
    }

   
    public IActionResult Create()
    {
        return View(new SaveMejoraViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveMejoraViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        
            var resultado = await _mejoraService.CreateAsync(model);
            
            if (resultado != null)
            {
                TempData["Success"] = "Mejora creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["Error"] = "No se pudo crear la mejora";
            return View(model);
        
    }

  
    public async Task<IActionResult> Edit(int id)
    {
        
        
            var mejora = await _mejoraService.GetByIdAsync(id);
            
            if (mejora == null)
            {
                TempData["Error"] = "Mejora no encontrada";
                return RedirectToAction(nameof(Index));
            }

            var model = new SaveMejoraViewModel
            {
                Nombre = mejora.Nombre,
                Descripcion = mejora.Descripcion
            };

            return View(model);
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaveMejoraViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        
            var resultado = await _mejoraService.UpdateAsync(id, model);
            
            if (resultado == null)
            {
                TempData["Error"] = "Mejora no encontrada";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Mejora actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        
    }


    public async Task<IActionResult> Delete(int id)
    {
        
            var mejora = await _mejoraService.GetByIdAsync(id);
            
            if (mejora == null)
            {
                TempData["Error"] = "Mejora no encontrada";
                return RedirectToAction(nameof(Index));
            }

            return View(mejora);
       
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        
            var resultado = await _mejoraService.DeleteAsync(id);

            if (resultado)
            {
                TempData["Success"] = "Mejora eliminada exitosamente";
            }
            else
            {
                TempData["Error"] = "Mejora no encontrada";
            }

            return RedirectToAction(nameof(Index));
       
        
    }
}
