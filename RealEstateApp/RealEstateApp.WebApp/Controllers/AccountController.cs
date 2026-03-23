using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Account;
using RealEstateApp.Identity.Entities;

namespace RealEstateApp.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IFileUploadService fileUploadService,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileUploadService = fileUploadService;
            _emailService = emailService;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        var path = returnUrl.Split('?', '#')[0];
                        if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) && !roles.Contains("Administrador"))
                        {
                            returnUrl = null;
                        }
                    }

                    if (roles.Contains("Administrador")) return RedirectToAction("Index", "Admin");
                    if (roles.Contains("Agente")) return RedirectToAction("Index", "Agente");
                    if (roles.Contains("Cliente")) return RedirectToAction("Index", "Home");
                }

                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.UsuarioOCorreo)
                       ?? await _userManager.FindByEmailAsync(model.UsuarioOCorreo);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Inicio de sesión inválido.");
                return View(model);
            }

          
            if (await _userManager.IsInRoleAsync(user, "Agente") && !user.EsActivo)
            {
                ModelState.AddModelError(string.Empty, "Su cuenta está pendiente de activación por un administrador.");
                return View(model);
            }

            
            if (await _userManager.IsInRoleAsync(user, "Cliente") && !user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Debe confirmar su correo electrónico antes de iniciar sesión.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RecordarMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Inicio de sesión inválido.");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                var path = returnUrl.Split('?', '#')[0];

                if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) && !roles.Contains("Administrador"))
                {
                    if (roles.Contains("Agente")) return RedirectToAction("Index", "Agente");
                    if (roles.Contains("Cliente")) return RedirectToAction("Index", "Home");
                    return RedirectToAction("Index", "Home");
                }
                return Redirect(returnUrl);
            }

            if (roles.Contains("Administrador")) return RedirectToAction("Index", "Admin");
            if (roles.Contains("Agente")) return RedirectToAction("Index", "Agente");
            return RedirectToAction("Index", "Home");
            
        }

       
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

           
               
                if (model.TipoUsuario != "Cliente" && model.TipoUsuario != "Agente")
                {
                    ModelState.AddModelError(string.Empty, "Tipo de usuario no válido");
                    return View(model);
                }

           
                var user = new ApplicationUser
                {
                    UserName = model.NombreUsuario,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    PhoneNumber = model.Telefono,
                    EsActivo = false, 
                    EmailConfirmed = false
                };

              
                if (model.FotoPerfil != null && _fileUploadService.IsValidImage(model.FotoPerfil))
                {
                    var rutaImagen = await _fileUploadService.UploadImageAsync(model.FotoPerfil, "usuarios");
                    user.UrlImagenPerfil = rutaImagen;
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                   
                    await _userManager.AddToRoleAsync(user, model.TipoUsuario);

                   
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: HttpContext.Request.Scheme);

                    if (model.TipoUsuario == "Cliente")
                    {
                       
                        if (!string.IsNullOrEmpty(callbackUrl))
                        {
                            await _emailService.SendConfirmationEmailAsync(user.Email!, callbackUrl);
                        }

                        TempData["Success"] = "Registro exitoso. Por favor revise su correo electrónico para confirmar su cuenta.";
                    }
                    else 
                    {
                        
                        TempData["Success"] = "Registro exitoso. Su cuenta será activada por un administrador.";
                    }

                    _logger.LogInformation("Nuevo usuario registrado: {Email} como {Role}", user.Email, model.TipoUsuario);
                    return RedirectToAction(nameof(Login));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
          
        }

       
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Link de confirmación inválido";
                return RedirectToAction("Index", "Home");
            }

            
            
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction("Index", "Home");
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                
                if (result.Succeeded)
                {
                  
                    user.EsActivo = true;
                    await _userManager.UpdateAsync(user);

                   
                    await _emailService.SendWelcomeEmailAsync(user.Email!, user.NombreCompleto);

                    TempData["Success"] = "¡Su cuenta ha sido confirmada exitosamente! Ya puede iniciar sesión.";
                    _logger.LogInformation("Email confirmado para usuario: {Email}", user.Email);
                }
                else
                {
                    TempData["Error"] = "Error al confirmar el correo electrónico";
                }

                return RedirectToAction(nameof(Login));
            
           
               
           
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión");
            TempData["Success"] = "Sesión cerrada exitosamente";
            return RedirectToAction("Index", "Home");
        }

       
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
