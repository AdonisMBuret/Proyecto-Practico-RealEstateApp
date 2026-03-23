using AutoMapper;
using RealEstateApp.Application.ViewModels.Catalogos;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;


public class TipoPropiedadService : ITipoPropiedadService
{
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly IMapper _mapper;

    public TipoPropiedadService(ITipoPropiedadRepository tipoPropiedadRepository, IMapper mapper)
    {
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _mapper = mapper;
    }

    public async Task<List<TipoPropiedadViewModel>> GetAllAsync()
    {
        var tipos = await _tipoPropiedadRepository.GetAllAsync();
        var viewModels = _mapper.Map<List<TipoPropiedadViewModel>>(tipos);


        foreach (var viewModel in viewModels)
        {
            viewModel.CantidadPropiedades = await _tipoPropiedadRepository.GetCantidadPropiedadesAsync(viewModel.Id);
        }

        return viewModels.OrderBy(t => t.Nombre).ToList();
    }

    public async Task<TipoPropiedadViewModel?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor que cero", nameof(id));

        var tipo = await _tipoPropiedadRepository.GetByIdAsync(id);
        
        if (tipo == null)
            return null;

        var viewModel = _mapper.Map<TipoPropiedadViewModel>(tipo);
        viewModel.CantidadPropiedades = await _tipoPropiedadRepository.GetCantidadPropiedadesAsync(tipo.Id);

        return viewModel;
    }

    public async Task<TipoPropiedadViewModel> CreateAsync(SaveTipoPropiedadViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

       
        var existe = await _tipoPropiedadRepository.ExisteConNombreAsync(viewModel.Nombre);
        if (existe)
        {
            throw new InvalidOperationException($"Ya existe un tipo de propiedad con el nombre '{viewModel.Nombre}'");
        }

        var tipo = _mapper.Map<TipoPropiedad>(viewModel);

        var tipoCreado = await _tipoPropiedadRepository.AddAsync(tipo);
        
        var resultado = _mapper.Map<TipoPropiedadViewModel>(tipoCreado);
        resultado.CantidadPropiedades = 0; 

        return resultado;
    }

    public async Task UpdateAsync(int id, SaveTipoPropiedadViewModel viewModel)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor que cero", nameof(id));

        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        var tipo = await _tipoPropiedadRepository.GetByIdAsync(id);
        if (tipo == null)
            throw new InvalidOperationException($"No se encontró el tipo de propiedad con ID {id}");

        
        var existe = await _tipoPropiedadRepository.ExisteConNombreAsync(viewModel.Nombre, id);
        if (existe)
        {
            throw new InvalidOperationException($"Ya existe otro tipo de propiedad con el nombre '{viewModel.Nombre}'");
        }

       
        _mapper.Map(viewModel, tipo);
        tipo.Id = id; 
        await _tipoPropiedadRepository.UpdateAsync(tipo);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID debe ser mayor que cero", nameof(id));

        var tipo = await _tipoPropiedadRepository.GetByIdAsync(id);
        if (tipo == null)
            throw new InvalidOperationException($"No se encontró el tipo de propiedad con ID {id}");

      
        var cantidadPropiedades = await _tipoPropiedadRepository.GetCantidadPropiedadesAsync(id);
        if (cantidadPropiedades > 0)
        {
            throw new InvalidOperationException($"No se puede eliminar el tipo de propiedad '{tipo.Nombre}' porque tiene {cantidadPropiedades} propiedad(es) asociada(s)");
        }

        await _tipoPropiedadRepository.DeleteAsync(tipo);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        if (id <= 0)
            return false;

        try
        {
            var tipo = await _tipoPropiedadRepository.GetByIdAsync(id);
            return tipo != null;
        }
        catch
        {
            return false;
        }
    }
}