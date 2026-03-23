using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.DTOs.Mantenimientos;
using RealEstateApp.Application.Features.TipoPropiedades.Commands.CreateTipoPropiedad;
using RealEstateApp.Application.Features.TipoPropiedades.Commands.UpdateTipoPropiedad;
using RealEstateApp.Application.Features.TipoPropiedades.Commands.DeleteTipoPropiedad;
using RealEstateApp.Application.Features.TipoPropiedades.Queries.GetAllTipoPropiedades;
using RealEstateApp.Application.Features.TipoPropiedades.Queries.GetTipoPropiedadById;

namespace RealEstateApp.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class TipoPropiedadesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TipoPropiedadesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TipoPropiedadApiDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]  
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TipoPropiedadApiDTO>>> List()
        {
            var result = await _mediator.Send(new GetAllTipoPropiedadesQuery());
            
            if (result == null || !result.Any())
            {
                return NoContent();
            }
            
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TipoPropiedadApiDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TipoPropiedadApiDTO>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new ProblemDetails
                {
                    Title = "Parámetros inválidos",
                    Detail = "El ID debe ser un número entero positivo",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(new GetTipoPropiedadByIdQuery { Id = id });
            
            if (result == null) 
                return NoContent();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(TipoPropiedadApiDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TipoPropiedadApiDTO>> Create([FromBody] CreateTipoPropiedadCommand command)
        {
            if (command == null)
                return BadRequest(new ProblemDetails
                {
                    Title = "Datos requeridos",
                    Detail = "Los datos del tipo de propiedad son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Actualiza un tipo de propiedad - SOLO envía los campos que quieres actualizar
        /// NO necesitas enviar el ID en el body, solo en la URL
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(TipoPropiedadApiDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TipoPropiedadApiDTO>> Update(
            int id, 
            [FromBody] UpdateTipoPropiedadCommand command)
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
                    Detail = "Los datos del tipo de propiedad son requeridos",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });

            // El ID viene de la ruta, NO del body
            command.Id = id;

            // Verificar si el tipo de propiedad existe antes de actualizar
            var exists = await _mediator.Send(new GetTipoPropiedadByIdQuery { Id = id });
            if (exists == null)
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Tipo de propiedad con ID {id} no encontrado",
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
            var exists = await _mediator.Send(new GetTipoPropiedadByIdQuery { Id = id });
            if (exists == null)
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = $"Tipo de propiedad con ID {id} no encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });

            await _mediator.Send(new DeleteTipoPropiedadCommand(id)); 
            return NoContent();
        }
    }
}
