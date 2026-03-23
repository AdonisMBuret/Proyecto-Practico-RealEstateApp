using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Propiedades.Commands.UpdatePropiedad;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Commands;
public class UpdatePropiedadCommandTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<ITipoVentaRepository> _mockTipoVentaRepository;
    private readonly Mock<IMejoraRepository> _mockMejoraRepository;
    private readonly UpdatePropiedadCommandHandler _handler;
    public UpdatePropiedadCommandTests()
    {
        _mockPropiedadRepository = new Mock<IPropiedadRepository>();
        _mockTipoPropiedadRepository = new Mock<ITipoPropiedadRepository>();
        _mockTipoVentaRepository = new Mock<ITipoVentaRepository>();
        _mockMejoraRepository = new Mock<IMejoraRepository>();
        _handler = new UpdatePropiedadCommandHandler(
            _mockPropiedadRepository.Object,
            _mockTipoPropiedadRepository.Object,
            _mockTipoVentaRepository.Object,
            _mockMejoraRepository.Object
        );
    }
    #region UpdatePropiedad Tests
    [Fact]
    public async Task Handle_When_ValidCommand_Should_UpdatePropiedadSuccessfully()
    {
        var command = CreateTestUpdatePropiedadCommand();
        var existingPropiedad = CreateTestExistingPropiedad(command.Id);
        var tipoPropiedad = CreateTestTipoPropiedad();
        var tipoVenta = CreateTestTipoVenta();
        _mockPropiedadRepository
            .Setup(x => x.GetDetalleByIdAsync(command.Id))
            .ReturnsAsync(existingPropiedad);
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.TipoPropiedadId))
            .ReturnsAsync(tipoPropiedad);
        _mockTipoVentaRepository
            .Setup(x => x.GetByIdAsync(command.TipoVentaId))
            .ReturnsAsync(tipoVenta);
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Mejora>());
        _mockPropiedadRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Propiedad>()))
            .Returns(Task.CompletedTask);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Mensaje.Should().Contain("exitosamente");
        _mockPropiedadRepository.Verify(x => x.GetDetalleByIdAsync(command.Id), Times.Once);
        _mockPropiedadRepository.Verify(x => x.UpdateAsync(It.IsAny<Propiedad>()), Times.Once);
        _mockTipoPropiedadRepository.Verify(x => x.GetByIdAsync(command.TipoPropiedadId), Times.Once);
        _mockTipoVentaRepository.Verify(x => x.GetByIdAsync(command.TipoVentaId), Times.Once);
    }
    [Fact]
    public async Task Handle_When_PropiedadNotFound_Should_ThrowKeyNotFoundException()
    {
        var command = CreateTestUpdatePropiedadCommand();
        _mockPropiedadRepository
            .Setup(x => x.GetDetalleByIdAsync(command.Id))
            .ReturnsAsync((Propiedad?)null);
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain($"No se encontró la propiedad con ID {command.Id}");
        _mockPropiedadRepository.Verify(x => x.GetDetalleByIdAsync(command.Id), Times.Once);
        _mockPropiedadRepository.Verify(x => x.UpdateAsync(It.IsAny<Propiedad>()), Times.Never);
    }
    [Fact]
    public async Task Handle_When_TipoPropiedadNotFound_Should_ThrowInvalidOperationException()
    {
        var command = CreateTestUpdatePropiedadCommand();
        var existingPropiedad = CreateTestExistingPropiedad(command.Id);
        _mockPropiedadRepository
            .Setup(x => x.GetDetalleByIdAsync(command.Id))
            .ReturnsAsync(existingPropiedad);
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.TipoPropiedadId))
            .ReturnsAsync((TipoPropiedad?)null);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("El tipo de propiedad seleccionado no existe");
    }
    [Fact]
    public async Task Handle_When_TipoVentaNotFound_Should_ThrowInvalidOperationException()
    {
        var command = CreateTestUpdatePropiedadCommand();
        var existingPropiedad = CreateTestExistingPropiedad(command.Id);
        var tipoPropiedad = CreateTestTipoPropiedad();
        _mockPropiedadRepository
            .Setup(x => x.GetDetalleByIdAsync(command.Id))
            .ReturnsAsync(existingPropiedad);
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.TipoPropiedadId))
            .ReturnsAsync(tipoPropiedad);
        _mockTipoVentaRepository
            .Setup(x => x.GetByIdAsync(command.TipoVentaId))
            .ReturnsAsync((TipoVenta?)null);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("El tipo de venta seleccionado no existe");
    }
    #endregion
    #region Helper Methods
    private static UpdatePropiedadCommand CreateTestUpdatePropiedadCommand() => new()
    {
        Id = 1,
        TipoPropiedadId = 1,
        TipoVentaId = 1,
        Precio = 6_000_000m,
        Descripcion = "Apartamento actualizado con mejores acabados",
        CantidadHabitaciones = 4,
        CantidadBanos = 3,
        TamanoEnMetros = 110,
        UrlImagenesExistentes = new List<string>(),
        UrlImagenesNuevas = new List<string>(),
        MejorasIds = new List<int>()
    };
    private static Propiedad CreateTestExistingPropiedad(int id) => new()
    {
        Id = id,
        Codigo = "PROP001",
        Precio = 5_200_000m,
        Descripcion = "Descripción original",
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        TamanoEnMetros = 95,
        AgenteId = "agente-123",
        TipoPropiedadId = 1,
        TipoVentaId = 1,
        Estado = EstadoPropiedad.Disponible,
        FechaCreacion = DateTime.UtcNow.AddDays(-30),
        Imagenes = new List<ImagenPropiedad>(),
        PropiedadesMejoras = new List<PropiedadMejora>()
    };
    private static TipoPropiedad CreateTestTipoPropiedad() => new()
    {
        Id = 1,
        Nombre = "Apartamento",
        Descripcion = "Apartamento residencial"
    };
    private static TipoVenta CreateTestTipoVenta() => new()
    {
        Id = 1,
        Nombre = "Venta",
        Descripcion = "Venta directa"
    };
    #endregion
}