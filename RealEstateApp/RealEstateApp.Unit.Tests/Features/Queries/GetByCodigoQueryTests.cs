using FluentAssertions;
using Moq;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Enums;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Queries;
public class GetByCodigoServiceTests
{
    private readonly Mock<IPropiedadService> _mockPropiedadService;
    public GetByCodigoServiceTests()
    {
        _mockPropiedadService = new Mock<IPropiedadService>();
    }
    [Fact]
    public async Task GetByCodigoAsync_When_CodigoExists_Should_ReturnPropiedad()
    {
        var codigo = "PROP001";
        var viewModel = CreateTestPropiedadViewModel(codigo);
        _mockPropiedadService
            .Setup(x => x.GetByCodigoAsync(codigo))
            .ReturnsAsync(viewModel);
        var result = await _mockPropiedadService.Object.GetByCodigoAsync(codigo);
        result.Should().NotBeNull();
        result!.Codigo.Should().Be(codigo);
        result.EstadoTexto.Should().Be(EstadoPropiedad.Disponible.ToString());
        _mockPropiedadService.Verify(x => x.GetByCodigoAsync(codigo), Times.Once);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetByCodigoAsync_When_CodigoInvalid_Should_ReturnNull(string invalidCodigo)
    {
        _mockPropiedadService
            .Setup(x => x.GetByCodigoAsync(invalidCodigo!))
            .ReturnsAsync((PropiedadViewModel?)null);
        var result = await _mockPropiedadService.Object.GetByCodigoAsync(invalidCodigo!);
        result.Should().BeNull();
        _mockPropiedadService.Verify(x => x.GetByCodigoAsync(invalidCodigo!), Times.Once);
    }
    private static PropiedadViewModel CreateTestPropiedadViewModel(string codigo) => new()
    {
        Id = 1,
        Codigo = codigo,
        Precio = 5_000_000m,
        Descripcion = "Propiedad de prueba",
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        TamanoEnMetros = 95,
        EstadoTexto = EstadoPropiedad.Disponible.ToString(),
        FechaCreacion = DateTime.UtcNow.AddDays(-7)
    };
}