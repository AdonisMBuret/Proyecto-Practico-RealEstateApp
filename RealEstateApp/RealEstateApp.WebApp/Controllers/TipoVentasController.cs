using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Catalogos;

namespace RealEstateApp.WebApp.Controllers;


[Authorize(Roles = "Administrador")]
public class TipoVentasController : Controller
{
    private readonly ITipoVentaService _tipoVentaService;
    private readonly ILogger<TipoVentasController> _logger;

    public TipoVentasController(
        ITipoVentaService tipoVentaService,
        ILogger<TipoVentasController> logger)
    {
        _tipoVentaService = tipoVentaService;
        _logger = logger;
    }

 
    public async Task<IActionResult> Index()
    {
        
            var tiposVenta = await _tipoVentaService.GetAllAsync();
            return View(tiposVenta);
       
    }


    public IActionResult Create()
    {
        return View(new SaveTipoVentaViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveTipoVentaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        
            await _tipoVentaService.CreateAsync(model);
            TempData["Success"] = "Tipo de venta creado exitosamente";
            return RedirectToAction(nameof(Index));
        
    }


    public async Task<IActionResult> Edit(int id)
    {
        
            var tipoVenta = await _tipoVentaService.GetByIdAsync(id);
            
            if (tipoVenta == null)
            {
                TempData["Error"] = "Tipo de venta no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var model = new SaveTipoVentaViewModel
            {
                Id = tipoVenta.Id,
                Nombre = tipoVenta.Nombre,
                Descripcion = tipoVenta.Descripcion
            };

            return View(model);
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaveTipoVentaViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Index));

        if (!ModelState.IsValid)
            return View(model);

        
            var result = await _tipoVentaService.UpdateAsync(id, model);
            
            if (result == null)
            {
                TempData["Error"] = "Tipo de venta no encontrado";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Tipo de venta actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        
    }

 
    public async Task<IActionResult> Delete(int id)
    {
        
            var tipoVenta = await _tipoVentaService.GetByIdAsync(id);
            
            if (tipoVenta == null)
            {
                TempData["Error"] = "Tipo de venta no encontrado";
                return RedirectToAction(nameof(Index));
            }

            return View(tipoVenta);
           
        
    }

    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        
            var result = await _tipoVentaService.DeleteAsync(id);

            if (result)
                TempData["Success"] = "Tipo de venta eliminado exitosamente";
            else
                TempData["Error"] = "Tipo de venta no encontrado";

            return RedirectToAction(nameof(Index));
       
    }
}
