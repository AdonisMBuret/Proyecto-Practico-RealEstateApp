using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Catalogos;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;


public class TipoVentaService : ITipoVentaService
{
    private readonly ITipoVentaRepository _tipoVentaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TipoVentaService> _logger;

    public TipoVentaService(
        ITipoVentaRepository tipoVentaRepository,
        IMapper mapper,
        ILogger<TipoVentaService> logger)
    {
        _tipoVentaRepository = tipoVentaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TipoVentaViewModel>> GetAllAsync()
    {
        try
        {
            var tiposVenta = await _tipoVentaRepository.GetAllAsync();
            var viewModels = _mapper.Map<List<TipoVentaViewModel>>(tiposVenta);
            
            
            foreach (var viewModel in viewModels)
            {
                viewModel.CantidadPropiedades = await _tipoVentaRepository.GetCantidadPropiedadesAsync(viewModel.Id);
            }
            
            return viewModels.OrderBy(x => x.Nombre).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de venta");
            return new List<TipoVentaViewModel>();
        }
    }

    public async Task<TipoVentaViewModel?> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0) return null;

            var tipoVenta = await _tipoVentaRepository.GetByIdAsync(id);
            if (tipoVenta == null) return null;

            var viewModel = _mapper.Map<TipoVentaViewModel>(tipoVenta);
            viewModel.CantidadPropiedades = await _tipoVentaRepository.GetCantidadPropiedadesAsync(id);
            
            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipo de venta por ID {TipoVentaId}", id);
            return null;
        }
    }

    public async Task<TipoVentaViewModel> CreateAsync(SaveTipoVentaViewModel viewModel)
    {
        try
        {
            if (await ExisteNombreAsync(viewModel.Nombre))
                throw new InvalidOperationException("Ya existe un tipo de venta con ese nombre");

            var tipoVenta = _mapper.Map<TipoVenta>(viewModel);
            
            var tipoVentaCreado = await _tipoVentaRepository.AddAsync(tipoVenta);
            
            _logger.LogInformation("Tipo de venta creado: {TipoVentaId} - {Nombre}", tipoVentaCreado.Id, tipoVentaCreado.Nombre);
            
            var resultado = _mapper.Map<TipoVentaViewModel>(tipoVentaCreado);
            resultado.CantidadPropiedades = 0; 
            
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de venta");
            throw;
        }
    }

    public async Task<TipoVentaViewModel?> UpdateAsync(int id, SaveTipoVentaViewModel viewModel)
    {
        try
        {
            if (await ExisteNombreAsync(viewModel.Nombre, id))
                throw new InvalidOperationException("Ya existe otro tipo de venta con ese nombre");

            var tipoVenta = await _tipoVentaRepository.GetByIdAsync(id);
            if (tipoVenta == null) return null;

            
            _mapper.Map(viewModel, tipoVenta);
            tipoVenta.Id = id; 
           
            await _tipoVentaRepository.UpdateAsync(tipoVenta);
            
            _logger.LogInformation("Tipo de venta actualizado: {TipoVentaId} - {Nombre}", id, tipoVenta.Nombre);
            
            var resultado = _mapper.Map<TipoVentaViewModel>(tipoVenta);
            resultado.CantidadPropiedades = await _tipoVentaRepository.GetCantidadPropiedadesAsync(id);
            
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de venta {TipoVentaId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var tipoVenta = await _tipoVentaRepository.GetByIdAsync(id);
            if (tipoVenta == null) return false;

           
            var cantidadPropiedades = await _tipoVentaRepository.GetCantidadPropiedadesAsync(id);
            if (cantidadPropiedades > 0)
                throw new InvalidOperationException($"No se puede eliminar el tipo de venta '{tipoVenta.Nombre}' porque tiene {cantidadPropiedades} propiedad(es) asociada(s)");

            await _tipoVentaRepository.DeleteAsync(tipoVenta);
            
            _logger.LogInformation("Tipo de venta eliminado: {TipoVentaId} - {Nombre}", id, tipoVenta.Nombre);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de venta {TipoVentaId}", id);
            throw;
        }
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
    {
        try
        {
            return await _tipoVentaRepository.ExistsWithNameAsync(nombre, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de nombre de tipo de venta");
            return false;
        }
    }
}
//commit de los services