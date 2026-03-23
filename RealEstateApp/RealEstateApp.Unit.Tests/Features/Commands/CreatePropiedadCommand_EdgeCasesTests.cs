using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Propiedades.Commands.CreatePropiedad;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Commands;
public class CreatePropiedadCommand_EdgeCasesTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<ITipoVentaRepository> _mockTipoVentaRepository;
    private readonly Mock<IMejoraRepository> _mockMejoraRepository;
    private readonly CreatePropiedadCommandHandler _handler;
    public CreatePropiedadCommand_EdgeCasesTests()
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
    [Fact]
    public async Task Handle_When_GeneratedCodeAlreadyExists_Should_RetryCodeGeneration()
    {
        var command = CreateTestCreatePropiedadCommand();
        SetupValidCatalogs();
        _mockPropiedadRepository
            .SetupSequence(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        var createdPropiedad = new Propiedad 
        { 
            Id = 1, 
            Codigo = "123456",
            AgenteId = command.AgenteId,
            Precio = command.Precio,
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = command.TipoPropiedadId,
            TipoVentaId = command.TipoVentaId,
            Descripcion = command.Descripcion,
            CantidadHabitaciones = command.CantidadHabitaciones,
            CantidadBanos = command.CantidadBanos,
            TamanoEnMetros = command.TamanoEnMetros,
            FechaCreacion = DateTime.UtcNow
        };
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(createdPropiedad)
            .Callback<Propiedad>(p => { p.Id = createdPropiedad.Id; p.Codigo = createdPropiedad.Codigo; });
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(1);
        result.Codigo.Should().Be("123456");
        _mockPropiedadRepository.Verify(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()), Times.AtLeast(2));
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.IsAny<Propiedad>()), Times.Once);
    }
    [Fact]
    public async Task Handle_When_TipoPropiedadNotExists_Should_ThrowInvalidOperationException()
    {
        var command = CreateTestCreatePropiedadCommand();
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.TipoPropiedadId))
            .ReturnsAsync((TipoPropiedad?)null);
        await FluentActions
            .Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*tipo de propiedad*");
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.IsAny<Propiedad>()), Times.Never);
    }
    [Fact]
    public async Task Handle_When_TipoVentaNotExists_Should_ThrowInvalidOperationException()
    {
        var command = CreateTestCreatePropiedadCommand();
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.TipoPropiedadId))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Apartamento" });
        _mockTipoVentaRepository
            .Setup(x => x.GetByIdAsync(command.TipoVentaId))
            .ReturnsAsync((TipoVenta?)null);
        await FluentActions
            .Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*tipo de venta*");
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.IsAny<Propiedad>()), Times.Never);
    }
    [Fact]
    public async Task Handle_When_ValidCommand_Should_CreatePropiedadWithCorrectData()
    {
        var command = CreateTestCreatePropiedadCommand();
        SetupValidCatalogs();
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
        var createdPropiedad = new Propiedad 
        { 
            Id = 1, 
            Codigo = "123456",
            AgenteId = command.AgenteId,
            Precio = command.Precio,
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = command.TipoPropiedadId,
            TipoVentaId = command.TipoVentaId
        };
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(createdPropiedad)
            .Callback<Propiedad>(p => { p.Id = createdPropiedad.Id; p.Codigo = createdPropiedad.Codigo; });
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(command.MejorasIds))
            .ReturnsAsync(new List<Mejora>
            {
                new() { Id = 1, Nombre = "Piscina" },
                new() { Id = 2, Nombre = "Gimnasio" }
            });
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(1);
        result.Codigo.Should().Be("123456");
        result.Mensaje.Should().Contain("exitosamente");
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.Is<Propiedad>(p =>
            p.AgenteId == command.AgenteId &&
            p.Precio == command.Precio &&
            p.Estado == EstadoPropiedad.Disponible &&
            p.TipoPropiedadId == command.TipoPropiedadId
        )), Times.Once);
    }
    [Fact]
    public async Task Handle_When_CommandWithImages_Should_AddImagenes()
    {
        var command = CreateTestCreatePropiedadCommand();
        command.UrlImagenes = new List<string> { "imagen1.jpg", "imagen2.jpg" };
        SetupValidCatalogs();
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
        var createdPropiedad = new Propiedad 
        { 
            Id = 1, 
            Codigo = "123456",
            Imagenes = new List<ImagenPropiedad>
            {
                new() { UrlImagen = "imagen1.jpg", EsPrincipal = true },
                new() { UrlImagen = "imagen2.jpg", EsPrincipal = false }
            }
        };
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(createdPropiedad);
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Mejora>());
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.Is<Propiedad>(p =>
            p.Imagenes != null &&
            p.Imagenes.Count == 2 &&
            p.Imagenes.Any(i => i.EsPrincipal) &&
            p.Imagenes.Any(i => i.UrlImagen == "imagen1.jpg")
        )), Times.Once);
    }
    private void SetupValidCatalogs()
    {
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Apartamento" });
        _mockTipoVentaRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new TipoVenta { Id = 1, Nombre = "Venta" });
        _mockMejoraRepository
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Mejora>());
    }
    private static CreatePropiedadCommand CreateTestCreatePropiedadCommand() => new()
    {
        AgenteId = "agente-123",
        TipoPropiedadId = 1,
        TipoVentaId = 1,
        Precio = 5_000_000m,
        TamanoEnMetros = 100.5,
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        Descripcion = "Test propiedad descripción con más de 10 caracteres",
        MejorasIds = new List<int> { 1, 2 },
        UrlImagenes = new List<string>()
    };
}