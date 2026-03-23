using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.DTOs.Account;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Identity.Entities;

namespace RealEstateApp.Api.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<CuentaController> _logger;

        public CuentaController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            ILogger<CuentaController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Login - Autenticarse y obtener token JWT
        /// </summary>
        /// <param name="request">Credenciales de login</param>
        /// <returns>Token JWT si las credenciales son correctas</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.EmailOrUsername)
                ?? await _userManager.FindByNameAsync(request.EmailOrUsername);

            if (user == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                });
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            if (!roles.Contains("Administrador") && !roles.Contains("Desarrollador"))
            {
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "No tiene permisos para acceder a la API. Solo usuarios Administrador o Desarrollador."
                });
            }

            var token = _jwtService.GenerateToken(
                user.Id,
                user.Email!,
                user.UserName!,
                roles
            );

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login exitoso",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Username = user.UserName,
                FullName = user.NombreCompleto,
                Roles = roles
            });
        }

        /// <summary>
        /// Registro de usuario Desarrollador
        /// </summary>
        /// <param name="request">Datos del nuevo usuario</param>
        /// <returns>Confirmación de registro</returns>
        [HttpPost("RegisterDeveloper")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegisterResponse>> RegisterDeveloper([FromBody] RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Cedula = request.Cedula,
                PhoneNumber = request.Telefono,
                EmailConfirmed = true, // Los usuarios del API se crean activos
                EsActivo = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            // Asignar rol Desarrollador
            await _userManager.AddToRoleAsync(user, "Desarrollador");

            return CreatedAtAction(nameof(Login), new RegisterResponse
            {
                Success = true,
                Message = $"Usuario Desarrollador {user.NombreCompleto} creado exitosamente",
                UserId = user.Id
            });
        }

        /// <summary>
        /// Registro de usuario Administrador (solo Admin puede crear Admin)
        /// </summary>
        /// <param name="request">Datos del nuevo usuario</param>
        /// <returns>Confirmación de registro</returns>
        [HttpPost("RegisterAdmin")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegisterResponse>> RegisterAdmin([FromBody] RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Cedula = request.Cedula,
                PhoneNumber = request.Telefono,
                EmailConfirmed = true,
                EsActivo = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            // Asignar rol Administrador
            await _userManager.AddToRoleAsync(user, "Administrador");

            return CreatedAtAction(nameof(Login), new RegisterResponse
            {
                Success = true,
                Message = $"Usuario Administrador {user.NombreCompleto} creado exitosamente",
                UserId = user.Id
            });
        }
    }
}
