using MediatR;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;


namespace RealEstateApp.Application.Features.Ofertas.Commands.CreateOferta;

public class CreateOfertaCommandHandler : IRequestHandler<CreateOfertaCommand, CreateOfertaResponse>
{
    private readonly IRepositoryAsync<Oferta> _ofertasRepository;
    private readonly IRepositoryAsync<Propiedad> _propiedadesRepository;

    public CreateOfertaCommandHandler(
        IRepositoryAsync<Oferta> ofertasRepository,
        IRepositoryAsync<Propiedad> propiedadesRepository)
    {
        _ofertasRepository = ofertasRepository;
        _propiedadesRepository = propiedadesRepository;
    }

    public async Task<CreateOfertaResponse> Handle(CreateOfertaCommand request, CancellationToken cancellationToken)
    {
        // Verificar que la propiedad existe y está disponible
        var propiedad = await _propiedadesRepository.GetByIdAsync(request.PropiedadId);
        if (propiedad == null)
        {
            return new CreateOfertaResponse
            {
                Success = false,
                Mensaje = "La propiedad no existe"
            };
        }

        if (propiedad.Estado != EstadoPropiedad.Disponible)
        {
            return new CreateOfertaResponse
            {
                Success = false,
                Mensaje = "Esta propiedad ya no está disponible"
            };
        }

        // Verificar que no haya una oferta aceptada
        var ofertas = await _ofertasRepository.GetAllAsync();
        var tieneOfertaAceptada = ofertas.Any(o => 
            o.PropiedadId == request.PropiedadId && 
            o.Estado == EstadoOferta.Aceptada);

        if (tieneOfertaAceptada)
        {
            return new CreateOfertaResponse
            {
                Success = false,
                Mensaje = "Esta propiedad ya tiene una oferta aceptada"
            };
        }

        var tieneOfertaPendiente = ofertas.Any(o => 
            o.PropiedadId == request.PropiedadId && 
            o.ClienteId == request.ClienteId && 
            o.Estado == EstadoOferta.Pendiente);

        if (tieneOfertaPendiente)
        {
            return new CreateOfertaResponse
            {
                Success = false,
                Mensaje = "Ya tienes una oferta pendiente para esta propiedad"
            };
        }

        var oferta = new Oferta
        {
            PropiedadId = request.PropiedadId,
            ClienteId = request.ClienteId,
            Monto = request.Monto,
            FechaCreacion = DateTime.Now,
            Estado = EstadoOferta.Pendiente
        };

        await _ofertasRepository.AddAsync(oferta);

        return new CreateOfertaResponse
        {
            Success = true,
            Id = oferta.Id,
            Mensaje = "Oferta enviada exitosamente. El agente la revisará pronto."
        };
    }
}
