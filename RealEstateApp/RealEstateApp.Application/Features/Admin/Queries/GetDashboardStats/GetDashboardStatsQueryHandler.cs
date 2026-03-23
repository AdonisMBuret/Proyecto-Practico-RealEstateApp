using MediatR;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Admin;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;

namespace RealEstateApp.Application.Features.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardViewModel>
{
    private readonly IRepositoryAsync<Propiedad> _propiedadesRepository;
    private readonly IRepositoryAsync<TipoPropiedad> _tipoPropiedadesRepository;
    private readonly IRepositoryAsync<TipoVenta> _tipoVentasRepository;
    private readonly IRepositoryAsync<Mejora> _mejorasRepository;
    private readonly IRepositoryAsync<Oferta> _ofertasRepository;
    private readonly IUserStatsService _userStatsService;

    public GetDashboardStatsQueryHandler(
        IRepositoryAsync<Propiedad> propiedadesRepository,
        IRepositoryAsync<TipoPropiedad> tipoPropiedadesRepository,
        IRepositoryAsync<TipoVenta> tipoVentasRepository,
        IRepositoryAsync<Mejora> mejorasRepository,
        IRepositoryAsync<Oferta> ofertasRepository,
        IUserStatsService userStatsService)
    {
        _propiedadesRepository = propiedadesRepository;
        _tipoPropiedadesRepository = tipoPropiedadesRepository;
        _tipoVentasRepository = tipoVentasRepository;
        _mejorasRepository = mejorasRepository;
        _ofertasRepository = ofertasRepository;
        _userStatsService = userStatsService;
    }

    public async Task<DashboardViewModel> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var propiedades = await _propiedadesRepository.GetAllAsync();

        var agentesStats = await _userStatsService.GetAgentesStatsAsync();
        var clientesStats = await _userStatsService.GetClientesStatsAsync();
        var desarrolladoresStats = await _userStatsService.GetDesarrolladoresStatsAsync();

        var tipoPropiedades = await _tipoPropiedadesRepository.GetAllAsync();
        var tipoVentas = await _tipoVentasRepository.GetAllAsync();
        var mejoras = await _mejorasRepository.GetAllAsync();
        var ofertas = await _ofertasRepository.GetAllAsync();

        return new DashboardViewModel
        {
            PropiedadesDisponibles = propiedades.Count(p => p.Estado == EstadoPropiedad.Disponible),
            PropiedadesVendidas = propiedades.Count(p => p.Estado == EstadoPropiedad.Vendida),

            AgentesActivos = agentesStats.activos,
            AgentesInactivos = agentesStats.inactivos,

            ClientesActivos = clientesStats.activos,
            ClientesInactivos = clientesStats.inactivos,

            DesarrolladoresActivos = desarrolladoresStats.activos,
            DesarrolladoresInactivos = desarrolladoresStats.inactivos,

            TotalTipoPropiedades = tipoPropiedades.Count,
            TotalTipoVentas = tipoVentas.Count,
            TotalMejoras = mejoras.Count,
            TotalOfertas = ofertas.Count
        };
    }
}
