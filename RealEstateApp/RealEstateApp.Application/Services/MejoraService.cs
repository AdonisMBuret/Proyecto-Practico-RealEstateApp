using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Catalogos;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;


public class MejoraService : IMejoraService
{
    private readonly IMejoraRepository _mejoraRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MejoraService> _logger;

    public MejoraService(
        IMejoraRepository mejoraRepository,
        IMapper mapper,
        ILogger<MejoraService> logger)
    {
        _mejoraRepository = mejoraRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MejoraViewModel>> GetAllAsync()
    {
        try
        {
            var mejoras = await _mejoraRepository.GetAllAsync();
            var viewModels = _mapper.Map<List<MejoraViewModel>>(mejoras);
            
            
            foreach (var viewModel in viewModels)
            {
                viewModel.CantidadPropiedades = await _mejoraRepository.GetCantidadPropiedadesAsync(viewModel.Id);
            }
            
            return viewModels.OrderBy(x => x.Nombre).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las mejoras");
            return new List<MejoraViewModel>();
        }
    }

    public async Task<MejoraViewModel?> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0) return null;

            var mejora = await _mejoraRepository.GetByIdAsync(id);
            if (mejora == null) return null;

            var viewModel = _mapper.Map<MejoraViewModel>(mejora);
            viewModel.CantidadPropiedades = await _mejoraRepository.GetCantidadPropiedadesAsync(id);
            
            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mejora por ID {MejoraId}", id);
            return null;
        }
    }

    public async Task<MejoraViewModel> CreateAsync(SaveMejoraViewModel viewModel)
    {
        try
        {
            if (await ExisteNombreAsync(viewModel.Nombre))
                throw new InvalidOperationException("Ya existe una mejora con ese nombre");

            var mejora = _mapper.Map<Mejora>(viewModel);
           
            var mejoraCreada = await _mejoraRepository.AddAsync(mejora);
            
            _logger.LogInformation("Mejora creada: {MejoraId} - {Nombre}", mejoraCreada.Id, mejoraCreada.Nombre);
            
            return _mapper.Map<MejoraViewModel>(mejoraCreada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear mejora");
            throw;
        }
    }

    public async Task<MejoraViewModel?> UpdateAsync(int id, SaveMejoraViewModel viewModel)
    {
        try
        {
            if (await ExisteNombreAsync(viewModel.Nombre, id))
                throw new InvalidOperationException("Ya existe otra mejora con ese nombre");

            var mejora = await _mejoraRepository.GetByIdAsync(id);
            if (mejora == null) return null;

            mejora.Nombre = viewModel.Nombre;
            mejora.Descripcion = viewModel.Descripcion;
            

            await _mejoraRepository.UpdateAsync(mejora);
            
            _logger.LogInformation("Mejora actualizada: {MejoraId} - {Nombre}", id, mejora.Nombre);
            
            return _mapper.Map<MejoraViewModel>(mejora);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar mejora {MejoraId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var mejora = await _mejoraRepository.GetByIdAsync(id);
            if (mejora == null) return false;

          
            var cantidadPropiedades = await _mejoraRepository.GetCantidadPropiedadesAsync(id);
            if (cantidadPropiedades > 0)
                throw new InvalidOperationException($"No se puede eliminar la mejora porque tiene {cantidadPropiedades} propiedades asociadas");

            await _mejoraRepository.DeleteAsync(mejora);
            
            _logger.LogInformation("Mejora eliminada: {MejoraId} - {Nombre}", id, mejora.Nombre);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar mejora {MejoraId}", id);
            throw;
        }
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
    {
        try
        {
            return await _mejoraRepository.ExistsWithNameAsync(nombre, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de nombre de mejora");
            return false;
        }
    }
}