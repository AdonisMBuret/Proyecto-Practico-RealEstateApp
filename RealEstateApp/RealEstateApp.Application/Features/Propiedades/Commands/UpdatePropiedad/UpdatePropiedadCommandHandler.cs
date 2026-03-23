using MediatR;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Propiedades.Commands.UpdatePropiedad;


public class UpdatePropiedadCommandHandler : IRequestHandler<UpdatePropiedadCommand, UpdatePropiedadResponse>
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly ITipoVentaRepository _tipoVentaRepository;
    private readonly IMejoraRepository _mejoraRepository;

    public UpdatePropiedadCommandHandler(
        IPropiedadRepository propiedadRepository,
        ITipoPropiedadRepository tipoPropiedadRepository,
        ITipoVentaRepository tipoVentaRepository,
        IMejoraRepository mejoraRepository)
    {
        _propiedadRepository = propiedadRepository;
        _tipoPropiedadRepository = tipoPropiedadRepository;
        _tipoVentaRepository = tipoVentaRepository;
        _mejoraRepository = mejoraRepository;
    }

    public async Task<UpdatePropiedadResponse> Handle(UpdatePropiedadCommand request, CancellationToken cancellationToken)
    {
        // Cambiar GetByIdWithDetailsAsync por GetDetalleByIdAsync
        var propiedad = await _propiedadRepository.GetDetalleByIdAsync(request.Id);

        if (propiedad == null)
            throw new KeyNotFoundException($"No se encontró la propiedad con ID {request.Id}");

        var tipoPropiedad = await _tipoPropiedadRepository.GetByIdAsync(request.TipoPropiedadId);
        if (tipoPropiedad == null)
            throw new InvalidOperationException("El tipo de propiedad seleccionado no existe");

        var tipoVenta = await _tipoVentaRepository.GetByIdAsync(request.TipoVentaId);
        if (tipoVenta == null)
            throw new InvalidOperationException("El tipo de venta seleccionado no existe");

        propiedad.TipoPropiedadId = request.TipoPropiedadId;
        propiedad.TipoVentaId = request.TipoVentaId;
        propiedad.Precio = request.Precio;
        propiedad.TamanoEnMetros = request.TamanoEnMetros;
        propiedad.CantidadHabitaciones = request.CantidadHabitaciones;
        propiedad.CantidadBanos = request.CantidadBanos;
        propiedad.Descripcion = request.Descripcion;

        if (request.UrlImagenesNuevas.Any())
        {
            // Eliminar imágenes antiguas que no están en la lista de existentes
            var imagenesAEliminar = propiedad.Imagenes
                .Where(i => !request.UrlImagenesExistentes.Contains(i.UrlImagen))
                .ToList();

            foreach (var imagen in imagenesAEliminar)
            {
                propiedad.Imagenes.Remove(imagen);
            }

            // Agregar nuevas imágenes
            foreach (var urlImagen in request.UrlImagenesNuevas)
            {
                propiedad.Imagenes.Add(new ImagenPropiedad
                {
                    UrlImagen = urlImagen,
                    EsPrincipal = !propiedad.Imagenes.Any() 
                });
            }
        }

     
        propiedad.PropiedadesMejoras.Clear();

        if (request.MejorasIds.Any())
        {
            var mejoras = await _mejoraRepository.GetByIdsAsync(request.MejorasIds);
            foreach (var mejora in mejoras)
            {
                propiedad.PropiedadesMejoras.Add(new PropiedadMejora
                {
                    PropiedadId = propiedad.Id,
                    MejoraId = mejora.Id
                });
            }
        }

        await _propiedadRepository.UpdateAsync(propiedad);

        return new UpdatePropiedadResponse
        {
            Success = true,
            Mensaje = "Propiedad actualizada exitosamente"
        };
    }
}
