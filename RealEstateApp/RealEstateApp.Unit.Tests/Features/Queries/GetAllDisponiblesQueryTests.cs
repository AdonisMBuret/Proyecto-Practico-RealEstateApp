using FluentAssertions;
using Moq;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Enums;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Queries;
public class GetAllDisponiblesServiceTests
{
    private readonly Mock<IPropiedadService> _mockPropiedadService;
    public GetAllDisponiblesServiceTests()
    {
        _mockPropiedadService = new Mock<IPropiedadService>();
    }
    [Fact]
    public async Task GetAllDisponiblesAsync_Should_ReturnOnlyAvailableProperties()
    {
        var propiedadesDisponibles = CreateTestDisponiblesPropiedades();
        _mockPropiedadService
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(propiedadesDisponibles);
        var result = await _mockPropiedadService.Object.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(p => p.EstadoTexto == EstadoPropiedad.Disponible.ToString()).Should().BeTrue();
        _mockPropiedadService.Verify(x => x.GetAllDisponiblesAsync(), Times.Once);
    }
    [Fact]
    public async Task GetAllDisponiblesAsync_When_NoPropertiesAvailable_Should_ReturnEmptyList()
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
    public async Task GetAllDisponiblesAsync_Should_ReturnPropertiesWithCorrectData()
    {
        var propiedadesDisponibles = CreateTestDisponiblesPropiedades();
        _mockPropiedadService
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(propiedadesDisponibles);
        var result = await _mockPropiedadService.Object.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        var firstProperty = result.First();
        firstProperty.Id.Should().Be(1);
        firstProperty.Codigo.Should().Be("PROP001");
        firstProperty.Precio.Should().Be(2_500_000m);
        firstProperty.EstadoTexto.Should().Be(EstadoPropiedad.Disponible.ToString());
        var secondProperty = result.Skip(1).First();
        secondProperty.Id.Should().Be(2);
        secondProperty.Codigo.Should().Be("PROP002");
        secondProperty.Precio.Should().Be(4_200_000m);
        secondProperty.EstadoTexto.Should().Be(EstadoPropiedad.Disponible.ToString());
    }
    private static List<PropiedadViewModel> CreateTestDisponiblesPropiedades() => new()
    {
        new PropiedadViewModel
        {
            Id = 1,
            Codigo = "PROP001",
            Precio = 2_500_000m,
            Descripcion = "Apartamento moderno",
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            TamanoEnMetros = 85,
            EstadoTexto = EstadoPropiedad.Disponible.ToString(),
            FechaCreacion = DateTime.UtcNow.AddDays(-10)
        },
        new PropiedadViewModel
        {
            Id = 2,
            Codigo = "PROP002",
            Precio = 4_200_000m,
            Descripcion = "Casa familiar amplia",
            CantidadHabitaciones = 4,
            CantidadBanos = 3,
            TamanoEnMetros = 150,
            EstadoTexto = EstadoPropiedad.Disponible.ToString(),
            FechaCreacion = DateTime.UtcNow.AddDays(-5)
        }
    };
}