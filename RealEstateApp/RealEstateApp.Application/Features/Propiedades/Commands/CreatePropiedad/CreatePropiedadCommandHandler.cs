using MediatR;

using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Propiedades.Commands.CreatePropiedad;


public class CreatePropiedadCommandHandler : IRequestHandler<CreatePropiedadCommand, CreatePropiedadResponse>
{
    private readonly IPropiedadRepository _propiedadRepository;
    private readonly ITipoPropiedadRepository _tipoPropiedadRepository;
    private readonly ITipoVentaRepository _tipoVentaRepository;
    private readonly IMejoraRepository _mejoraRepository;

    public CreatePropiedadCommandHandler(
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

    public async Task<CreatePropiedadResponse> Handle(CreatePropiedadCommand request, CancellationToken cancellationToken)
    {
        var tipoPropiedad = await _tipoPropiedadRepository.GetByIdAsync(request.TipoPropiedadId);
        if (tipoPropiedad == null)
            throw new InvalidOperationException("El tipo de propiedad seleccionado no existe");

        var tipoVenta = await _tipoVentaRepository.GetByIdAsync(request.TipoVentaId);
        if (tipoVenta == null)
            throw new InvalidOperationException("El tipo de venta seleccionado no existe");

        // Generar código único de 6 dígitos
        var codigo = await GenerarCodigoUnicoAsync();

       
        var propiedad = new Propiedad
        {
            Codigo = codigo,
            TipoPropiedadId = request.TipoPropiedadId,
            TipoVentaId = request.TipoVentaId,
            Precio = request.Precio,
            TamanoEnMetros = request.TamanoEnMetros,
            CantidadHabitaciones = request.CantidadHabitaciones,
            CantidadBanos = request.CantidadBanos,
            Descripcion = request.Descripcion,
            AgenteId = request.AgenteId,
            Estado = EstadoPropiedad.Disponible,
            FechaCreacion = DateTime.UtcNow
        };

        foreach (var urlImagen in request.UrlImagenes)
        {
            propiedad.Imagenes.Add(new ImagenPropiedad
            {
                UrlImagen = urlImagen,
                EsPrincipal = propiedad.Imagenes.Count == 0 
            });
        }

        if (request.MejorasIds.Any())
        {
            var mejoras = await _mejoraRepository.GetByIdsAsync(request.MejorasIds);
            foreach (var mejora in mejoras)
            {
                propiedad.PropiedadesMejoras.Add(new PropiedadMejora
                {
                    MejoraId = mejora.Id
                });
            }
        }

        // Guardar en la base de datos
        await _propiedadRepository.AddAsync(propiedad);

        return new CreatePropiedadResponse
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            Success = true,
            Mensaje = "Propiedad creada exitosamente"
        };
    }

   
    private async Task<string> GenerarCodigoUnicoAsync()
    {
        string codigo;
        bool existe;

        do
        {
            //  aquí se generaa código aleatorio de 6 dígitos
            var random = new Random();
            codigo = random.Next(100000, 999999).ToString();

            existe = await _propiedadRepository.ExisteCodigoAsync(codigo);

        } while (existe);

        return codigo;
    }
}
