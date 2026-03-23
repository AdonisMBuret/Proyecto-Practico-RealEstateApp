using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Application.Features.Mejoras.Commands.CreateMejora;
using RealEstateApp.Application.Features.Mejoras.Commands.DeleteMejora;
using RealEstateApp.Application.Features.Mejoras.Commands.UpdateMejora;
using RealEstateApp.Application.Features.Mejoras.Queries.GetAllMejoras;
using RealEstateApp.Application.Features.Mejoras.Queries.GetMejoraById;

namespace RealEstateApp.Api.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class MejorasController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MejorasController> _logger;

        public MejorasController(IMediator mediator, ILogger<MejorasController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las mejoras disponibles
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<MejoraApiDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var mejoras = await _mediator.Send(new GetAllMejorasQuery());
            
            if (mejoras == null || !mejoras.Any())
                return NoContent();

            return Ok(mejoras);
        }

        /// <summary>
        /// Obtiene una mejora por su ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MejoraApiDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID debe ser un número entero positivo",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            var mejora = await _mediator.Send(new GetMejoraByIdQuery { Id = id });
            
            if (mejora == null)
                return NoContent();
            
            return Ok(mejora);
        }

        /// <summary>
        /// Crea una nueva mejora
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(MejoraApiDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateMejoraCommand command)
        {
            if (command == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Datos requeridos",
                    Detail = "Los datos de la mejora son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Actualiza una mejora - SOLO envía los campos que quieres actualizar
        /// NO necesitas enviar el ID en el body, solo en la URL
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(MejoraApiDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(
            int id, 
            [FromBody] UpdateMejoraCommand command)
        {
            if (id <= 0)
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID debe ser un número entero positivo",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            if (command == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Datos requeridos",
                    Detail = "Los datos de la mejora son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            // El ID viene de la ruta, NO del body
            command.Id = id;

            // Verificar si la mejora existe antes de actualizar
            var exists = await _mediator.Send(new GetMejoraByIdQuery { Id = id });
            if (exists == null)
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Mejora con ID {id} no encontrada",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Elimina una mejora
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID debe ser un número entero positivo",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            // Verificar si existe antes de eliminar
            var exists = await _mediator.Send(new GetMejoraByIdQuery { Id = id });
            if (exists == null)
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Mejora con ID {id} no encontrada",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });

            await _mediator.Send(new DeleteMejoraCommand(id));
            return NoContent();
        }
    }
}


