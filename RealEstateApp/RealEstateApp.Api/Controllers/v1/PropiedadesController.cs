using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Application.DTOs.Propiedades;
using RealEstateApp.Application.Features.Propiedades.Queries.GetAllPropiedades;
using RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadByCodigo;
using RealEstateApp.Application.Features.Propiedades.Queries.GetPropiedadById;
using RealEstateApp.Application.ViewModels.Propiedades;

namespace RealEstateApp.Api.Controllers.v1
{
    /// <summary>
    /// Controlador de Propiedades
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Desarrollador")]
    public class PropiedadesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PropiedadesController> _logger;

        public PropiedadesController(
            IMediator mediator,
            ILogger<PropiedadesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el listado de todas las propiedades
        /// </summary>
        /// <returns>Lista de propiedades en formato JSON</returns>
        /// <response code="200">Retorna el listado de propiedades</response>
        /// <response code="204">No existen propiedades</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<PropiedadDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PropiedadDTO>>> List()
        {
            _logger.LogInformation("Obteniendo listado de todas las propiedades");

            var propiedades = await _mediator.Send(new GetAllPropiedadesQuery { SoloDisponibles = false });

            if (propiedades == null || !propiedades.Any())
            {
                _logger.LogInformation("No se encontraron propiedades");
                return NoContent();
            }

            // Mapear a DTO
            var propiedadesDTO = MapViewModelToDTO(propiedades);

            _logger.LogInformation("Se obtuvieron {Count} propiedades", propiedadesDTO.Count);
            return Ok(propiedadesDTO);
        }

        /// <summary>
        /// Obtiene una propiedad por su ID
        /// </summary>
        /// <param name="id">ID de la propiedad</param>
        /// <returns>Datos de la propiedad</returns>
        /// <response code="200">Retorna los datos de la propiedad</response>
        /// <response code="204">No existe la propiedad</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PropiedadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PropiedadDTO>> GetById(int id)
        {
            // Validar ID
            if (id <= 0)
                throw new ArgumentException("El ID debe ser un número entero positivo", nameof(id));

            _logger.LogInformation("Obteniendo propiedad con ID: {Id}", id);

            var propiedad = await _mediator.Send(new GetPropiedadByIdQuery(id));

            if (propiedad == null)
            {
                _logger.LogInformation("No se encontró la propiedad con ID: {Id}", id);
                return NoContent();
            }

            // Mapear a DTO
            var propiedadDTO = MapViewModelToDTO(propiedad);

            _logger.LogInformation("Propiedad {Id} obtenida exitosamente", id);
            return Ok(propiedadDTO);
        }

        /// <summary>
        /// Obtiene una propiedad por su código
        /// </summary>
        /// <param name="codigo">Código de la propiedad (6 dígitos)</param>
        /// <returns>Datos de la propiedad</returns>
        /// <response code="200">Retorna los datos de la propiedad</response>
        /// <response code="204">No existe la propiedad</response>
        /// <response code="400">Código inválido</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("code/{codigo}")]
        [ProducesResponseType(typeof(PropiedadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PropiedadDTO>> GetByCode(string codigo)
        {
            // Validar que el código no esté vacío
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código de la propiedad es requerido", nameof(codigo));

            // Validar formato del código
            if (codigo.Length < 3)
                throw new ArgumentException("El código de la propiedad debe tener al menos 3 caracteres", nameof(codigo));

            _logger.LogInformation("Obteniendo propiedad con código: {Codigo}", codigo);

            var propiedad = await _mediator.Send(new GetPropiedadByCodigoQuery(codigo));

            if (propiedad == null)
            {
                _logger.LogInformation("No se encontró la propiedad con código: {Codigo}", codigo);
                return NoContent();
            }

            // Mapear a DTO
            var propiedadDTO = MapViewModelToDTO(propiedad);

            _logger.LogInformation("Propiedad con código {Codigo} obtenida exitosamente", codigo);
            return Ok(propiedadDTO);
        }

        #region Helper Methods

        private PropiedadDTO MapViewModelToDTO(PropiedadViewModel viewModel)
        {
            return new PropiedadDTO
            {
                Id = viewModel.Id,
                Codigo = viewModel.Codigo,
                TipoPropiedad = viewModel.TipoPropiedad,
                TipoVenta = viewModel.TipoVenta,
                Precio = viewModel.Precio,
                TamanoMetros = (decimal)viewModel.TamanoEnMetros,
                CantidadHabitaciones = viewModel.CantidadHabitaciones,
                CantidadBanos = viewModel.CantidadBanos,
                Descripcion = viewModel.Descripcion,
                Mejoras = viewModel.Mejoras ?? new List<string>(), 
                NombreAgente = viewModel.AgenteNombre, 
                IdAgente = viewModel.AgenteId,
                EstadoPropiedad = viewModel.EstadoTexto ?? "Disponible" 
            };
        }

        private List<PropiedadDTO> MapViewModelToDTO(List<PropiedadViewModel> viewModels)
        {
            return viewModels.Select(vm => MapViewModelToDTO(vm)).ToList();
        }

        #endregion
    }
}
