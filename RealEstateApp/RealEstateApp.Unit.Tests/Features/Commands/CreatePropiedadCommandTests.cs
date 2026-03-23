using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Propiedades.Commands.CreatePropiedad;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Commands;
public class CreatePropiedadCommandTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<ITipoVentaRepository> _mockTipoVentaRepository;
    private readonly Mock<IMejoraRepository> _mockMejoraRepository;
    private readonly CreatePropiedadCommandHandler _handler;
    public CreatePropiedadCommandTests()
    {
        _mockPropiedadRepository = new Mock<IPropiedadRepository>();
        _mockTipoPropiedadRepository = new Mock<ITipoPropiedadRepository>();
        _mockTipoVentaRepository = new Mock<ITipoVentaRepository>();
        _mockMejoraRepository = new Mock<IMejoraRepository>();
        _handler = new CreatePropiedadCommandHandler(
            _mockPropiedadRepository.Object,
            _mockTipoPropiedadRepository.Object,
            _mockTipoVentaRepository.Object,
            _mockMejoraRepository.Object
        );
    }
    #region CreatePropiedad Tests
    [Fact]
    public async Task Handle_When_ValidCommand_Should_CreatePropiedadSuccessfully()
    {
        var command = CreateTestCreatePropiedadCommand();
        SetupValidCatalogs();
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync((Propiedad p) =>
            {
                p.Id = 1;
                return p;
            });
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(command.MejorasIds))
            .ReturnsAsync(new List<Mejora>());
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(1);
        result.Codigo.Should().NotBeNullOrEmpty();
        result.Mensaje.Should().NotBeNullOrEmpty();
        result.Mensaje.ToLowerInvariant().Should().Contain("exitosamente");
        _mockTipoPropiedadRepository.Verify(x => x.GetByIdAsync(command.TipoPropiedadId), Times.Once);
        _mockTipoVentaRepository.Verify(x => x.GetByIdAsync(command.TipoVentaId), Times.Once);
        _mockPropiedadRepository.Verify(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()), Times.AtLeast(1));
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.IsAny<Propiedad>()), Times.Once);
    }
    [Fact]
    public async Task Handle_When_InvalidPrecio_HandlerDoesNotValidatePriceAnd_CreatesPropiedad()
    {
        var command = CreateTestCreatePropiedadCommand();
        command.Precio = -1000m;
        SetupValidCatalogs();
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync((Propiedad p) =>
            {
                p.Id = 1;
                return p;
            });
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(command.MejorasIds))
            .ReturnsAsync(new List<Mejora>());
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(1);
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.IsAny<Propiedad>()), Times.Once);
    }
    [Fact]
    public async Task Handle_When_InvalidAgenteId_HandlerDoesNotValidateAgenteId_And_CreatesPropiedad()
    {
        var command = CreateTestCreatePropiedadCommand();
        command.AgenteId = string.Empty;
        SetupValidCatalogs();
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync((Propiedad p) =>
            {
                p.Id = 1;
                return p;
            });
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(command.MejorasIds))
            .ReturnsAsync(new List<Mejora>());
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(1);
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.IsAny<Propiedad>()), Times.Once);
    }
    [Fact]
    public async Task Handle_When_RepositoryThrowsException_Should_PropagateException()
    {
        var command = CreateTestCreatePropiedadCommand();
        SetupValidCatalogs();
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    #endregion
    #region Helper Methods
    private void SetupValidCatalogs()
    {
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Apartamento" });
        _mockTipoVentaRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new TipoVenta { Id = 1, Nombre = "Venta" });
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>() ))
            .ReturnsAsync(new List<Mejora>());
    }
    private static CreatePropiedadCommand CreateTestCreatePropiedadCommand() => new()
    {
        AgenteId = "agente-123",
        TipoPropiedadId = 1,
        TipoVentaId = 1,
        Precio = 5_200_000m,
        Descripcion = "Apartamento en la Capital con excelente ubicación",
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        TamanoEnMetros = 95,
        MejorasIds = new List<int> { 1, 2, 3 },
        UrlImagenes = new List<string>()
    };
    #endregion
}