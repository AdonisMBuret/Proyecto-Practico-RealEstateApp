using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.DTOs.Agentes;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Application.Features.Agentes.Queries.GetAllAgentes;
using RealEstateApp.Application.Features.Agentes.Queries.GetAgenteById;
using RealEstateApp.Application.Features.Propiedades.Queries.GetAllPropiedades;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Identity.Entities;

namespace RealEstateApp.Api.Controllers.v1
{
    /// <summary>
    /// Controlador de Agentes
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class AgentesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AgentesController> _logger;

        public AgentesController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            ILogger<AgentesController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el listado de todos los agentes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<AgenteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AgenteDTO>>> List()
        {
            _logger.LogInformation("Obteniendo listado de todos los agentes");

            var agentes = await _mediator.Send(new GetAllAgentesQuery { SoloActivos = false });

            if (agentes == null || !agentes.Any())
            {
                _logger.LogInformation("No se encontraron agentes");
                return NoContent();
            }

            var agentesDTO = MapViewModelToDTO(agentes);

            _logger.LogInformation("Se obtuvieron {Count} agentes", agentesDTO.Count);
            return Ok(agentesDTO);
        }

        /// <summary>
        /// Obtiene un agente por su ID
        /// </summary>
        /// <param name="id">ID del agente</param>
        /// <returns>Datos del agente</returns>
        /// <response code="200">Retorna los datos del agente</response>
        /// <response code="204">No existe el agente</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AgenteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AgenteDTO>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID del agente es requerido",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            _logger.LogInformation("Obteniendo agente con ID: {Id}", id);

            var agente = await _mediator.Send(new GetAgenteByIdQuery(id));

            if (agente == null)
            {
                _logger.LogInformation("No se encontró el agente con ID: {Id}", id);
                return NoContent();
            }

            var agenteDTO = MapViewModelToDTO(agente);

            _logger.LogInformation("Agente {Id} obtenido exitosamente", id);
            return Ok(agenteDTO);
        }

        /// <summary>
        /// Obtiene las propiedades de un agente específico
        /// </summary>
        /// <param name="id">ID del agente</param>
        /// <returns>Lista de propiedades del agente</returns>
        /// <response code="200">Retorna el listado de propiedades del agente</response>
        /// <response code="204">No existen propiedades para el agente</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}/properties")]
        [ProducesResponseType(typeof(List<PropiedadDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PropiedadDTO>>> GetAgentProperty(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID del agente es requerido",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            _logger.LogInformation("Obteniendo propiedades del agente con ID: {Id}", id);

            // Verificar que el agente existe
            var agente = await _mediator.Send(new GetAgenteByIdQuery(id));
            if (agente == null)
            {
                _logger.LogInformation("No se encontró el agente con ID: {Id}", id);
                return NoContent();
            }

            // Obtener todas las propiedades y filtrar por agente
            var todasPropiedades = await _mediator.Send(new GetAllPropiedadesQuery { SoloDisponibles = false });
            var propiedadesAgente = todasPropiedades.Where(p => p.AgenteId == id).ToList();

            if (!propiedadesAgente.Any())
            {
                _logger.LogInformation("No se encontraron propiedades para el agente con ID: {Id}", id);
                return NoContent();
            }

            var propiedadesDTO = propiedadesAgente.Select(p => new PropiedadDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                TipoPropiedad = p.TipoPropiedad,
                TipoVenta = p.TipoVenta,
                Precio = p.Precio,
                TamanoMetros = (decimal)p.TamanoEnMetros, 
                CantidadHabitaciones = p.CantidadHabitaciones,
                CantidadBanos = p.CantidadBanos,
                Descripcion = p.Descripcion,
                Mejoras = p.Mejoras ?? new List<string>(), 
                NombreAgente = p.AgenteNombre, 
                IdAgente = p.AgenteId,
                EstadoPropiedad = p.EstadoTexto ?? "Disponible" 
            }).ToList();

            _logger.LogInformation("Se obtuvieron {Count} propiedades del agente {Id}", propiedadesDTO.Count, id);
            return Ok(propiedadesDTO);
        }

        /// <summary>
        /// Cambia el estado (activo/inactivo) de un agente
        /// </summary>
        /// <param name="id">ID del agente</param>
        /// <param name="request">Objeto con el nuevo estado</param>
        /// <returns>Sin contenido si fue exitoso</returns>
        /// <response code="204">Estado cambiado exitosamente</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado (solo Administrador)</response>
        /// <response code="404">Agente no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID del agente es requerido",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            if (request == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Datos requeridos",
                    Detail = "Los datos del cambio de estado son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            if (!ModelState.IsValid)
                return BadRequest(new ProblemDetails
                {
                    Title = "Datos inválidos",
                    Detail = "Los datos proporcionados no son válidos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            _logger.LogInformation("Cambiando estado del agente {Id} a {Estado}", id, request.EsActivo ? "Activo" : "Inactivo");

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                _logger.LogInformation("No se encontró el agente con ID: {Id}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Agente con ID {id} no encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            if (!roles.Contains("Agente"))
                return BadRequest(new ProblemDetails
                {
                    Title = "Operación inválida",
                    Detail = "El usuario especificado no es un agente",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            usuario.EsActivo = request.EsActivo;
            var result = await _userManager.UpdateAsync(usuario);

            if (!result.Succeeded)
            {
                _logger.LogError("Error al cambiar el estado del agente {Id}: {Errors}", id, string.Join("; ", result.Errors.Select(e => e.Description)));
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Error interno",
                    Detail = $"No se pudo cambiar el estado del agente: {string.Join("; ", result.Errors.Select(e => e.Description))}",
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request.Path
                });
            }

            _logger.LogInformation("Estado del agente {Id} cambiado exitosamente", id);
            return NoContent();
        }

        #region Helper Methods

        private AgenteDTO MapViewModelToDTO(AgenteViewModel viewModel)
        {
            return new AgenteDTO
            {
                Id = viewModel.Id,
                Nombre = viewModel.Nombre,
                Apellido = viewModel.Apellido,
                CantidadPropiedades = viewModel.CantidadPropiedades,
                Correo = viewModel.Email,
                Telefono = viewModel.Telefono
            };
        }

        private List<AgenteDTO> MapViewModelToDTO(List<AgenteViewModel> viewModels)
        {
            return viewModels.Select(vm => MapViewModelToDTO(vm)).ToList();
        }

        #endregion
    }

    /// <summary>
    /// Modelo para cambio de estado del agente
    /// </summary>
    public class ChangeStatusRequest
    {
        public bool EsActivo { get; set; }
    }
}
