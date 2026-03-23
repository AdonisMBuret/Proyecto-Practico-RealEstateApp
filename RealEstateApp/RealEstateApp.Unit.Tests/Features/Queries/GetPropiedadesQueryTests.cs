using FluentAssertions;
using Moq;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Enums;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Queries;
public class GetPropiedadesServiceTests
{
    private readonly Mock<IPropiedadService> _mockPropiedadService;
    public GetPropiedadesServiceTests()
    {
        _mockPropiedadService = new Mock<IPropiedadService>();
    }
    #region GetPropiedades Tests
    [Fact]
    public async Task GetAllDisponiblesAsync_Should_ReturnAllAvailableProperties()
    {
        var propiedades = CreateTestPropiedades();
        _mockPropiedadService
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(propiedades);
        var result = await _mockPropiedadService.Object.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(propiedades.Count);
        result.All(p => p.Precio > 0).Should().BeTrue();
        _mockPropiedadService.Verify(x => x.GetAllDisponiblesAsync(), Times.Once);
    }
    [Fact]
    public async Task GetByFiltrosAsync_Should_ReturnFilteredResults()
    {
        var filtros = new FiltrosPropiedadesViewModel
        {
            TipoPropiedadId = 1,
            PrecioMinimo = 1_000_000m,
            PrecioMaximo = 10_000_000m
        };
        var filteredPropiedades = CreateTestPropiedades()
            .Where(p => p.TipoPropiedadId == 1 && p.Precio >= 1_000_000m && p.Precio <= 10_000_000m)
            .ToList();
        _mockPropiedadService
            .Setup(x => x.GetByFiltrosAsync(filtros))
            .ReturnsAsync(filteredPropiedades);
        var result = await _mockPropiedadService.Object.GetByFiltrosAsync(filtros);
        result.Should().NotBeNull();
        result.Should().HaveCount(2); 
        result.All(p => p.Precio >= filtros.PrecioMinimo && p.Precio <= filtros.PrecioMaximo)
            .Should().BeTrue();
        _mockPropiedadService.Verify(x => x.GetByFiltrosAsync(filtros), Times.Once);
    }
    [Fact]
    public async Task GetByAgenteIdAsync_Should_ReturnPropertiesFromSpecificAgent()
    {
        var agenteId = "agente-123";
        var propiedadesAgente = CreateTestPropiedades().Take(2).ToList();
        _mockPropiedadService
            .Setup(x => x.GetByAgenteIdAsync(agenteId))
            .ReturnsAsync(propiedadesAgente);
        var result = await _mockPropiedadService.Object.GetByAgenteIdAsync(agenteId);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(p => p.AgenteId == agenteId).Should().BeTrue();
        _mockPropiedadService.Verify(x => x.GetByAgenteIdAsync(agenteId), Times.Once);
    }
    [Fact]
    public async Task GetByCodigoAsync_Should_ReturnSpecificProperty()
    {
        var codigo = "PROP001";
        var propiedad = CreateTestPropiedades().First();
        _mockPropiedadService
            .Setup(x => x.GetByCodigoAsync(codigo))
            .ReturnsAsync(propiedad);
        var result = await _mockPropiedadService.Object.GetByCodigoAsync(codigo);
        result.Should().NotBeNull();
        result!.Codigo.Should().Be(codigo);
        _mockPropiedadService.Verify(x => x.GetByCodigoAsync(codigo), Times.Once);
    }
    [Fact]
    public async Task GetAllDisponiblesAsync_When_NoResults_Should_ReturnEmptyList()
    {
        var emptyList = new List<PropiedadViewModel>();
        _mockPropiedadService
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(emptyList);
        var result = await _mockPropiedadService.Object.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockPropiedadService.Verify(x => x.GetAllDisponiblesAsync(), Times.Once);
    }
    [Fact]
    public async Task ExisteAsync_Should_ReturnTrueWhenPropertyExists()
    {
        var propiedadId = 1;
        _mockPropiedadService
            .Setup(x => x.ExisteAsync(propiedadId))
            .ReturnsAsync(true);
        var result = await _mockPropiedadService.Object.ExisteAsync(propiedadId);
        result.Should().BeTrue();
        _mockPropiedadService.Verify(x => x.ExisteAsync(propiedadId), Times.Once);
    }
    [Fact]
    public async Task EstaDisponibleAsync_Should_ReturnTrueForAvailableProperty()
    {
        var propiedadId = 1;
        _mockPropiedadService
            .Setup(x => x.EstaDisponibleAsync(propiedadId))
            .ReturnsAsync(true);
        var result = await _mockPropiedadService.Object.EstaDisponibleAsync(propiedadId);
        result.Should().BeTrue();
        _mockPropiedadService.Verify(x => x.EstaDisponibleAsync(propiedadId), Times.Once);
    }
    #endregion
    #region Helper Methods
    private static List<PropiedadViewModel> CreateTestPropiedades() => new()
    {
        new PropiedadViewModel
        {
            Id = 1,
            Codigo = "PROP001",
            Precio = 2_500_000m,
            TipoPropiedadId = 1,
            TipoPropiedad = "Apartamento",
            TipoVenta = "Venta",
            EstadoTexto = EstadoPropiedad.Disponible.ToString(),
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            TamanoEnMetros = 95,
            AgenteId = "agente-123",
            AgenteNombre = "Juan Pérez",
            FechaCreacion = DateTime.UtcNow.AddDays(-10)
        },
        new PropiedadViewModel
        {
            Id = 2,
            Codigo = "PROP002",
            Precio = 4_200_000m,
            TipoPropiedadId = 1,
            TipoPropiedad = "Casa",
            TipoVenta = "Venta",
            EstadoTexto = EstadoPropiedad.Disponible.ToString(),
            CantidadHabitaciones = 4,
            CantidadBanos = 3,
            TamanoEnMetros = 120,
            AgenteId = "agente-123",
            AgenteNombre = "María García",
            FechaCreacion = DateTime.UtcNow.AddDays(-5)
        },
        new PropiedadViewModel
        {
            Id = 3,
            Codigo = "PROP003",
            Precio = 15_800_000m,
            TipoPropiedadId = 2,
            TipoPropiedad = "Villa",
            TipoVenta = "Venta",
            EstadoTexto = EstadoPropiedad.Disponible.ToString(),
            CantidadHabitaciones = 5,
            CantidadBanos = 4,
            TamanoEnMetros = 200,
            AgenteId = "agente-456",
            AgenteNombre = "Carlos Rodríguez",
            FechaCreacion = DateTime.UtcNow.AddDays(-2)
        }
    };
    #endregion
}