using MediatR;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Ofertas.Commands.AceptarOferta;

public class AceptarOfertaCommandHandler : IRequestHandler<AceptarOfertaCommand, AceptarOfertaResponse>
{
    private readonly IRepositoryAsync<Oferta> _ofertasRepository;
    private readonly IRepositoryAsync<Propiedad> _propiedadesRepository;

    public AceptarOfertaCommandHandler(
        IRepositoryAsync<Oferta> ofertasRepository,
        IRepositoryAsync<Propiedad> propiedadesRepository)
    {
        _ofertasRepository = ofertasRepository;
        _propiedadesRepository = propiedadesRepository;
    }

    public async Task<AceptarOfertaResponse> Handle(AceptarOfertaCommand request, CancellationToken cancellationToken)
    {
        // Obtener la oferta
        var oferta = await _ofertasRepository.GetByIdAsync(request.OfertaId);
        if (oferta == null)
        {
            return new AceptarOfertaResponse
            {
                Success = false,
                Mensaje = "La oferta no existe"
            };
        }

        // Verificar la propiedad y que pertenece al agente
        var propiedad = await _propiedadesRepository.GetByIdAsync(oferta.PropiedadId);
        if (propiedad == null)
        {
            return new AceptarOfertaResponse
            {
                Success = false,
                Mensaje = "La propiedad no existe"
            };
        }

        if (propiedad.AgenteId != request.AgenteId)
        {
            return new AceptarOfertaResponse
            {
                Success = false,
                Mensaje = "No tienes permiso para aceptar ofertas de esta propiedad"
            };
        }

        // Verificar que la oferta esté pendiente
        if (oferta.Estado != EstadoOferta.Pendiente)
        {
            return new AceptarOfertaResponse
            {
                Success = false,
                Mensaje = "Esta oferta ya fue procesada"
            };
        }

        oferta.Estado = EstadoOferta.Aceptada;
        await _ofertasRepository.UpdateAsync(oferta);

        var todasOfertas = await _ofertasRepository.GetAllAsync();
        var ofertasPendientes = todasOfertas.Where(o =>
            o.PropiedadId == oferta.PropiedadId &&
            o.Id != oferta.Id &&
            o.Estado == EstadoOferta.Pendiente).ToList();

        foreach (var ofertaPendiente in ofertasPendientes)
        {
            ofertaPendiente.Estado = EstadoOferta.Rechazada;
            await _ofertasRepository.UpdateAsync(ofertaPendiente);
        }

        propiedad.Estado = EstadoPropiedad.Vendida;
        await _propiedadesRepository.UpdateAsync(propiedad);

        return new AceptarOfertaResponse
        {
            Success = true,
            Id = oferta.Id,
            Mensaje = "Oferta aceptada. La propiedad ha sido marcada como vendida."
        };
    }
}
