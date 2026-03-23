using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Application.Features.TipoVentas.Commands.CreateTipoVenta;
using RealEstateApp.Application.Features.TipoVentas.Commands.DeleteTipoVenta;
using RealEstateApp.Application.Features.TipoVentas.Commands.UpdateTipoVenta;
using RealEstateApp.Application.Features.TipoVentas.Queries.GetAllTipoVentas;
using RealEstateApp.Application.Features.TipoVentas.Queries.GetTipoVentaById;

namespace RealEstateApp.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class TipoVentasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TipoVentasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TipoVentaApiDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TipoVentaApiDTO>>> List()
        {
            var result = await _mediator.Send(new GetAllTipoVentasQuery());
            
            if (result == null || !result.Any())
            {
                return NoContent();
            }
            
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TipoVentaApiDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TipoVentaApiDTO>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID debe ser un número entero positivo",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(new GetTipoVentaByIdQuery { Id = id });
            
            if (result == null) 
                return NoContent();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(TipoVentaApiDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TipoVentaApiDTO>> Create([FromBody] CreateTipoVentaCommand command)
        {
            if (command == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Datos requeridos",
                    Detail = "Los datos del tipo de venta son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Actualiza un tipo de venta - SOLO envía los campos que quieres actualizar
        /// NO necesitas enviar el ID en el body, solo en la URL
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(TipoVentaApiDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TipoVentaApiDTO>> Update(
            int id,
            [FromBody] UpdateTipoVentaCommand command)
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
                    Detail = "Los datos del tipo de venta son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            // El ID viene de la ruta, NO del body
            command.Id = id;

            // Verificar si el tipo de venta existe antes de actualizar
            var exists = await _mediator.Send(new GetTipoVentaByIdQuery { Id = id });
            if (exists == null)
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Tipo de venta con ID {id} no encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(int id)
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
            var exists = await _mediator.Send(new GetTipoVentaByIdQuery { Id = id });
            if (exists == null)
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Tipo de venta con ID {id} no encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });

            await _mediator.Send(new DeleteTipoVentaCommand(id));
            return NoContent();
        }
    }
}
