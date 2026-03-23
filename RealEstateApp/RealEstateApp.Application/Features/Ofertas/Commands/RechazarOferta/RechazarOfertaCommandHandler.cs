using MediatR;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Ofertas.Commands.RechazarOferta;

public class RechazarOfertaCommandHandler : IRequestHandler<RechazarOfertaCommand, RechazarOfertaResponse>
{
    private readonly IRepositoryAsync<Oferta> _ofertasRepository;
    private readonly IRepositoryAsync<Propiedad> _propiedadesRepository;

    public RechazarOfertaCommandHandler(
        IRepositoryAsync<Oferta> ofertasRepository,
        IRepositoryAsync<Propiedad> propiedadesRepository)
    {
        _ofertasRepository = ofertasRepository;
        _propiedadesRepository = propiedadesRepository;
    }

    public async Task<RechazarOfertaResponse> Handle(RechazarOfertaCommand request, CancellationToken cancellationToken)
    {
        // Obtener la oferta
        var oferta = await _ofertasRepository.GetByIdAsync(request.OfertaId);
        if (oferta == null)
        {
            return new RechazarOfertaResponse
            {
                Success = false,
                Mensaje = "La oferta no existe"
            };
        }

        var propiedad = await _propiedadesRepository.GetByIdAsync(oferta.PropiedadId);
        if (propiedad == null)
        {
            return new RechazarOfertaResponse
            {
                Success = false,
                Mensaje = "La propiedad no existe"
            };
        }

        if (propiedad.AgenteId != request.AgenteId)
        {
            return new RechazarOfertaResponse
            {
                Success = false,
                Mensaje = "No tienes permiso para rechazar ofertas de esta propiedad"
            };
        }

        if (oferta.Estado != EstadoOferta.Pendiente)
        {
            return new RechazarOfertaResponse
            {
                Success = false,
                Mensaje = "Esta oferta ya fue procesada"
            };
        }

        oferta.Estado = EstadoOferta.Rechazada;
        await _ofertasRepository.UpdateAsync(oferta);

        return new RechazarOfertaResponse
        {
            Success = true,
            Id = oferta.Id,
            Mensaje = "Oferta rechazada exitosamente"
        };
    }
}
