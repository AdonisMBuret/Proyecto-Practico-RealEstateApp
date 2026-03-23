using AutoMapper;
using MediatR;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Ofertas.Queries.GetOfertasByCliente;

public class GetOfertasByClienteQueryHandler : IRequestHandler<GetOfertasByClienteQuery, List<OfertaViewModel>>
{
    private readonly IRepositoryAsync<Oferta> _ofertasRepository;
    private readonly IRepositoryAsync<Propiedad> _propiedadesRepository;
    private readonly IMapper _mapper;

    public GetOfertasByClienteQueryHandler(
        IRepositoryAsync<Oferta> ofertasRepository,
        IRepositoryAsync<Propiedad> propiedadesRepository,
        IMapper mapper)
    {
        _ofertasRepository = ofertasRepository;
        _propiedadesRepository = propiedadesRepository;
        _mapper = mapper;
    }

    public async Task<List<OfertaViewModel>> Handle(GetOfertasByClienteQuery request, CancellationToken cancellationToken)
    {
        var ofertas = await _ofertasRepository.GetAllAsync();
        var ofertasCliente = ofertas.Where(o => o.ClienteId == request.ClienteId);

        if (request.PropiedadId.HasValue)
        {
            ofertasCliente = ofertasCliente.Where(o => o.PropiedadId == request.PropiedadId.Value);
        }

        var propiedades = await _propiedadesRepository.GetAllAsync();

        var viewModels = ofertasCliente.Select(o =>
        {
            var propiedad = propiedades.FirstOrDefault(p => p.Id == o.PropiedadId);
            return new OfertaViewModel
            {
                Id = o.Id,
                MontoOferta = o.Monto, 
                Estado = (int)o.Estado, 
                EstadoTexto = o.Estado.ToString(),
                FechaCreacion = o.FechaCreacion,
                PropiedadId = o.PropiedadId,
                CodigoPropiedad = propiedad?.Codigo ?? "",
                ClienteId = o.ClienteId,
                ClienteNombre = "" 
            };
        }).OrderByDescending(o => o.FechaCreacion).ToList();

        return viewModels;
    }
}
