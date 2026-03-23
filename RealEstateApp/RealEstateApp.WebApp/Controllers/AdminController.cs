using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Admin;
using RealEstateApp.Identity.Entities;
using System.Security.Claims;

namespace RealEstateApp.WebApp.Controllers;


[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPropiedadService _propiedadService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        IPropiedadService propiedadService,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _propiedadService = propiedadService;
        _logger = logger;
    }

    
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Admin.Index - IsAuthenticated: {IsAuthenticated}, User: {User}", 
            User.Identity?.IsAuthenticated, User.Identity?.Name);

        var roles = User.Claims
                        .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

        _logger.LogInformation("Admin.Index - Claims Roles: {Roles}", string.Join(", ", roles));

        var dashboard = new AdminDashboardViewModel();
        var propiedadesStats = await _propiedadService.GetEstadisticasPropiedadesAsync();
        dashboard.PropiedadesDisponibles = propiedadesStats.Disponibles;
        dashboard.PropiedadesVendidas = propiedadesStats.Vendidas;

        var agentes = await _userManager.GetUsersInRoleAsync("Agente");
        dashboard.AgentesActivos = agentes.Count(u => u.EsActivo);
        dashboard.AgentesInactivos = agentes.Count(u => !u.EsActivo);

        var clientes = await _userManager.GetUsersInRoleAsync("Cliente");
        dashboard.ClientesActivos = clientes.Count(u => u.EsActivo);
        dashboard.ClientesInactivos = clientes.Count(u => !u.EsActivo);

        var desarrolladores = await _userManager.GetUsersInRoleAsync("Desarrollador");
        dashboard.DesarrolladoresActivos = desarrolladores.Count(u => u.EsActivo);
        dashboard.DesarrolladoresInactivos = desarrolladores.Count(u => !u.EsActivo);

        return View(dashboard);
        
    }

    #region Gestión de Agentes (Ya implementado)
    
    
    public async Task<IActionResult> Agentes()
    {
        
            var agentes = await _userManager.GetUsersInRoleAsync("Agente");
            
            var agentesViewModel = new List<AdminAgenteViewModel>();
            
            foreach (var agente in agentes)
            {
                
                var cantidadPropiedades = await _propiedadService.GetCantidadPropiedadesByAgenteAsync(agente.Id);
                
                agentesViewModel.Add(new AdminAgenteViewModel
                {
                    Id = agente.Id,
                    Nombre = agente.Nombre, 
                    Apellido = agente.Apellido,
                    Email = agente.Email!,
                    UserName = agente.UserName!,
                    IsActive = agente.EsActivo, 
                    CantidadPropiedades = cantidadPropiedades,
                    FechaCreacion = agente.FechaCreacion 
                });
            }

            return View(agentesViewModel.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre).ToList());
        
    }

   
    [HttpPost]
    public async Task<IActionResult> ToggleAgenteEstado(string agenteId)
    {
        
            var agente = await _userManager.FindByIdAsync(agenteId);
            if (agente == null)
                return Json(new { success = false, message = "Agente no encontrado" });

           
            agente.EsActivo = !agente.EsActivo;
            var result = await _userManager.UpdateAsync(agente);

            if (result.Succeeded)
            {
                var mensaje = agente.EsActivo ? "Agente activado exitosamente" : "Agente desactivado exitosamente"; // ? CORREGIDO
                _logger.LogInformation("Estado del agente {AgenteId} cambiado a {Estado}", agenteId, agente.EsActivo ? "Activo" : "Inactivo"); // ? CORREGIDO
                return Json(new { success = true, isActive = agente.EsActivo, message = mensaje }); // ? CORREGIDO
            }
            
            return Json(new { success = false, message = "Error al actualizar el estado del agente" });
        
    }

   
    public async Task<IActionResult> EliminarAgente(string id)
    {
        
            var agente = await _userManager.FindByIdAsync(id);
            if (agente == null)
            {
                TempData["Error"] = "Agente no encontrado";
                return RedirectToAction(nameof(Agentes));
            }

            var cantidadPropiedades = await _propiedadService.GetCantidadPropiedadesByAgenteAsync(id);
            
            var viewModel = new EliminarAgenteViewModel
            {
                Id = agente.Id,
                NombreCompleto = $"{agente.Nombre} {agente.Apellido}", 
                Email = agente.Email!,
                CantidadPropiedades = cantidadPropiedades
            };

            return View(viewModel);
        
    }

  
    [HttpPost, ActionName("EliminarAgente")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAgenteConfirmado(string id)
    {
        
            var agente = await _userManager.FindByIdAsync(id);
            if (agente == null)
            {
                TempData["Error"] = "Agente no encontrado";
                return RedirectToAction(nameof(Agentes));
            }

            
            await _propiedadService.DeleteAllByAgenteAsync(id);

            
            var result = await _userManager.DeleteAsync(agente);

            if (result.Succeeded)
            {
                _logger.LogWarning("Agente eliminado: {AgenteId} - {NombreCompleto}", id, $"{agente.Nombre} {agente.Apellido}"); // ? CORREGIDO
                TempData["Success"] = "Agente y todas sus propiedades eliminados exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el agente";
            }

            return RedirectToAction(nameof(Agentes));
        
    }
    
    #endregion

    #region ? MANTENIMIENTO DE ADMINISTRADORES - FUNCIONALIDAD CORREGIDA

   
    public async Task<IActionResult> Administradores()
    {
        
            var administradores = await _userManager.GetUsersInRoleAsync("Administrador");
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var administradoresViewModel = administradores.Select(admin => new AdministradorViewModel
            {
                Id = admin.Id,
                Nombre = admin.Nombre, 
                Apellido = admin.Apellido, 
                Cedula = admin.Cedula,
                Email = admin.Email!,
                UserName = admin.UserName!,
                IsActive = admin.EsActivo, 
                FechaCreacion = admin.FechaCreacion 
            }).OrderBy(a => a.Apellido).ThenBy(a => a.Nombre).ToList();

            ViewBag.CurrentUserId = currentUserId;
            return View(administradoresViewModel);
        
    }

    
    public IActionResult CrearAdministrador()
    {
        return View(new SaveAdministradorViewModel());
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearAdministrador(SaveAdministradorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con este email");
                return View(model);
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("UserName", "Ya existe un usuario con este nombre de usuario");
                return View(model);
            }

            var administrador = new ApplicationUser
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Cedula = model.Cedula,
                Email = model.Email,
                UserName = model.UserName,
                EmailConfirmed = true,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(administrador, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(administrador, "Administrador");
                _logger.LogInformation("Administrador creado: {UserId} - {UserName}", administrador.Id, administrador.UserName);
                TempData["Success"] = "Administrador creado exitosamente";
                return RedirectToAction(nameof(Administradores));
            }

       foreach (var error in result.Errors)
        {
            _logger.LogWarning("Error al crear administrador: {Description}", error.Description);
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Excepción al crear administrador");
      
        ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el administrador. Revisa los logs.");
        return View(model);
    }
    }

    
    public async Task<IActionResult> EditarAdministrador(string id)
    {
        
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
            {
                TempData["Error"] = "No puede editar su propio usuario desde esta sección";
                return RedirectToAction(nameof(Administradores));
            }

            var administrador = await _userManager.FindByIdAsync(id);
            if (administrador == null || !await _userManager.IsInRoleAsync(administrador, "Administrador"))
            {
                TempData["Error"] = "Administrador no encontrado";
                return RedirectToAction(nameof(Administradores));
            }

            var model = new SaveAdministradorViewModel
            {
                Id = administrador.Id,
                Nombre = administrador.Nombre, 
                Apellido = administrador.Apellido, 
                Cedula = administrador.Cedula,
                Email = administrador.Email!,
                UserName = administrador.UserName!
            };

            return View(model);
          
        
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarAdministrador(string id, SaveAdministradorViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Administradores));

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == currentUserId)
        {
            TempData["Error"] = "No puede editar su propio usuario desde esta sección";
            return RedirectToAction(nameof(Administradores));
        }

        if (!ModelState.IsValid)
            return View(model);

        
            var administrador = await _userManager.FindByIdAsync(id);
            if (administrador == null || !await _userManager.IsInRoleAsync(administrador, "Administrador"))
            {
                TempData["Error"] = "Administrador no encontrado";
                return RedirectToAction(nameof(Administradores));
            }

           
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != id)
            {
                ModelState.AddModelError("Email", "Ya existe otro usuario con este email");
                return View(model);
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByUsername != null && existingUserByUsername.Id != id)
            {
                ModelState.AddModelError("UserName", "Ya existe otro usuario con este nombre de usuario");
                return View(model);
            }

           
            administrador.Nombre = model.Nombre; 
            administrador.Apellido = model.Apellido; 
            administrador.Cedula = model.Cedula;
            administrador.Email = model.Email;
            administrador.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(administrador);

            if (result.Succeeded)
            {
               
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(administrador);
                    var passwordResult = await _userManager.ResetPasswordAsync(administrador, token, model.Password);
                    
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }

                _logger.LogInformation("Administrador actualizado: {UserId} - {UserName}", administrador.Id, administrador.UserName);
                TempData["Success"] = "Administrador actualizado exitosamente";
                return RedirectToAction(nameof(Administradores));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        
    }

   
    [HttpPost]
    public async Task<IActionResult> ToggleAdministradorEstado(string administradorId)
    {
        
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (administradorId == currentUserId)
            {
                return Json(new { success = false, message = "No puede cambiar el estado de su propio usuario" });
            }

            var administrador = await _userManager.FindByIdAsync(administradorId);
            if (administrador == null || !await _userManager.IsInRoleAsync(administrador, "Administrador"))
            {
                return Json(new { success = false, message = "Administrador no encontrado" });
            }

            
            administrador.EsActivo = !administrador.EsActivo; 
            var result = await _userManager.UpdateAsync(administrador);

            if (result.Succeeded)
            {
                var mensaje = administrador.EsActivo ? "Administrador activado exitosamente" : "Administrador desactivado exitosamente"; // ? CORREGIDO
                _logger.LogInformation("Estado del administrador {AdministradorId} cambiado a {Estado}", administradorId, administrador.EsActivo ? "Activo" : "Inactivo"); // ? CORREGIDO
                return Json(new { success = true, isActive = administrador.EsActivo, message = mensaje }); // ? CORREGIDO
            }
            
            return Json(new { success = false, message = "Error al actualizar el estado del administrador" });
        
    }

    #endregion

    #region ? MANTENIMIENTO DE DESARROLLADORES - FUNCIONALIDAD CORREGIDA

   
    public async Task<IActionResult> Desarrolladores()
    {
        
            var desarrolladores = await _userManager.GetUsersInRoleAsync("Desarrollador");
            
            var desarrolladoresViewModel = desarrolladores.Select(dev => new DesarrolladorViewModel
            {
                Id = dev.Id,
                Nombre = dev.Nombre, 
                Apellido = dev.Apellido, 
                Cedula = dev.Cedula,
                Email = dev.Email!,
                UserName = dev.UserName!,
                IsActive = dev.EsActivo, 
                FechaCreacion = dev.FechaCreacion 
            }).OrderBy(d => d.Apellido).ThenBy(d => d.Nombre).ToList();

            return View(desarrolladoresViewModel);
        
    }

    
    public IActionResult CrearDesarrollador()
    {
        return View(new SaveDesarrolladorViewModel());
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDesarrollador(SaveDesarrolladorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        
            
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con este email");
                return View(model);
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("UserName", "Ya existe un usuario con este nombre de usuario");
                return View(model);
            }

           
            var desarrollador = new ApplicationUser
            {
                Nombre = model.Nombre, 
                Apellido = model.Apellido, 
                Cedula = model.Cedula,
                Email = model.Email,
                UserName = model.UserName,
                EmailConfirmed = true,
                EsActivo = true, 
                FechaCreacion = DateTime.UtcNow 
            };

            var result = await _userManager.CreateAsync(desarrollador, model.Password);

            if (result.Succeeded)
            {
                
                await _userManager.AddToRoleAsync(desarrollador, "Desarrollador");
                
                _logger.LogInformation("Desarrollador creado: {UserId} - {UserName}", desarrollador.Id, desarrollador.UserName);
                TempData["Success"] = "Desarrollador creado exitosamente";
                return RedirectToAction(nameof(Desarrolladores));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        
    }

    
    public async Task<IActionResult> EditarDesarrollador(string id)
    {
       
            var desarrollador = await _userManager.FindByIdAsync(id);
            if (desarrollador == null || !await _userManager.IsInRoleAsync(desarrollador, "Desarrollador"))
            {
                TempData["Error"] = "Desarrollador no encontrado";
                return RedirectToAction(nameof(Desarrolladores));
            }

            var model = new SaveDesarrolladorViewModel
            {
                Id = desarrollador.Id,
                Nombre = desarrollador.Nombre, 
                Apellido = desarrollador.Apellido,
                Cedula = desarrollador.Cedula,
                Email = desarrollador.Email!,
                UserName = desarrollador.UserName!
            };

            return View(model);
        
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarDesarrollador(string id, SaveDesarrolladorViewModel model)
    {
        if (id != model.Id)
            return RedirectToAction(nameof(Desarrolladores));

        if (!ModelState.IsValid)
            return View(model);

        
            var desarrollador = await _userManager.FindByIdAsync(id);
            if (desarrollador == null || !await _userManager.IsInRoleAsync(desarrollador, "Desarrollador"))
            {
                TempData["Error"] = "Desarrollador no encontrado";
                return RedirectToAction(nameof(Desarrolladores));
            }

           
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != id)
            {
                ModelState.AddModelError("Email", "Ya existe otro usuario con este email");
                return View(model);
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByUsername != null && existingUserByUsername.Id != id)
            {
                ModelState.AddModelError("UserName", "Ya existe otro usuario con este nombre de usuario");
                return View(model);
            }

           
            desarrollador.Nombre = model.Nombre; 
            desarrollador.Apellido = model.Apellido; 
            desarrollador.Cedula = model.Cedula;
            desarrollador.Email = model.Email;
            desarrollador.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(desarrollador);

            if (result.Succeeded)
            {
                
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(desarrollador);
                    var passwordResult = await _userManager.ResetPasswordAsync(desarrollador, token, model.Password);
                    
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }

                _logger.LogInformation("Desarrollador actualizado: {UserId} - {UserName}", desarrollador.Id, desarrollador.UserName);
                TempData["Success"] = "Desarrollador actualizado exitosamente";
                return RedirectToAction(nameof(Desarrolladores));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
       
    }

   
    [HttpPost]
    public async Task<IActionResult> ToggleDesarrolladorEstado(string desarrolladorId)
    {
       
            var desarrollador = await _userManager.FindByIdAsync(desarrolladorId);
            if (desarrollador == null || !await _userManager.IsInRoleAsync(desarrollador, "Desarrollador"))
            {
                return Json(new { success = false, message = "Desarrollador no encontrado" });
            }

           
            desarrollador.EsActivo = !desarrollador.EsActivo;
            var result = await _userManager.UpdateAsync(desarrollador);

            if (result.Succeeded)
            {
                var mensaje = desarrollador.EsActivo ? "Desarrollador activado exitosamente" : "Desarrollador desactivado exitosamente"; // ? CORREGIDO
                _logger.LogInformation("Estado del desarrollador {DesarrolladorId} cambiado a {Estado}", desarrolladorId, desarrollador.EsActivo ? "Activo" : "Inactivo"); // ? CORREGIDO
                return Json(new { success = true, isActive = desarrollador.EsActivo, message = mensaje }); // ? CORREGIDO
            }
            
            return Json(new { success = false, message = "Error al actualizar el estado del desarrollador" });
         
        
    }

    #endregion
}
