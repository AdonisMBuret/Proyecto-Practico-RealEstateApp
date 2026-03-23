using System;
using System.Threading;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Ofertas.Commands.AceptarOferta;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Commands;
public class AceptarOfertaCommandTests
{
    private readonly Mock<IRepositoryAsync<Oferta>> _mockOfertaRepository;
    private readonly Mock<IRepositoryAsync<Propiedad>> _mockPropiedadRepository;
    private readonly AceptarOfertaCommandHandler _handler;
    public AceptarOfertaCommandTests()
    {
        _mockOfertaRepository = new Mock<IRepositoryAsync<Oferta>>();
        _mockPropiedadRepository = new Mock<IRepositoryAsync<Propiedad>>();
        _handler = new AceptarOfertaCommandHandler(
            _mockOfertaRepository.Object,
            _mockPropiedadRepository.Object
        );
    }
    [Fact]
    public async Task Handle_When_ValidCommand_Should_AcceptOfferSuccessfully()
    {
        var command = new AceptarOfertaCommand
        {
            OfertaId = 1,
            AgenteId = "agente-123"
        };
        var oferta = new Oferta
        {
            Id = 1,
            PropiedadId = 5,
            ClienteId = "cliente-456",
            Monto = 2_500_000m,
            Estado = EstadoOferta.Pendiente
        };
        var propiedad = new Propiedad
        {
            Id = 5,
            AgenteId = "agente-123",
            Estado = EstadoPropiedad.Disponible
        };
        var todasOfertas = new List<Oferta> { oferta };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(command.OfertaId))
            .ReturnsAsync(oferta);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(oferta.PropiedadId))
            .ReturnsAsync(propiedad);
        _mockOfertaRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(todasOfertas);
        _mockOfertaRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Oferta>()))
            .Returns(Task.CompletedTask);
        _mockPropiedadRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Propiedad>()))
            .Returns(Task.CompletedTask);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().Be(1);
        result.Mensaje.Should().Contain("aceptada");
        _mockOfertaRepository.Verify(x => x.UpdateAsync(It.Is<Oferta>(o => 
            o.Id == 1 && 
            o.Estado == EstadoOferta.Aceptada
        )), Times.Once);
        _mockPropiedadRepository.Verify(x => x.UpdateAsync(It.Is<Propiedad>(p => 
            p.Id == 5 && 
            p.Estado == EstadoPropiedad.Vendida
        )), Times.Once);
    }
    [Fact]
    public async Task Handle_When_OfertaNotFound_Should_ReturnFailureResponse()
    {
        var command = new AceptarOfertaCommand
        {
            OfertaId = 999,
            AgenteId = "agente-123"
        };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(command.OfertaId))
            .ReturnsAsync((Oferta?)null);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Mensaje.Should().Contain("no existe");
    }
    [Fact]
    public async Task Handle_When_PropiedadNotFound_Should_ReturnFailureResponse()
    {
        var command = new AceptarOfertaCommand
        {
            OfertaId = 1,
            AgenteId = "agente-123"
        };
        var oferta = new Oferta
        {
            Id = 1,
            PropiedadId = 999,
            Estado = EstadoOferta.Pendiente
        };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(command.OfertaId))
            .ReturnsAsync(oferta);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(oferta.PropiedadId))
            .ReturnsAsync((Propiedad?)null);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Mensaje.Should().Contain("propiedad no existe");
    }
    [Fact]
    public async Task Handle_When_UnauthorizedAgent_Should_ReturnFailureResponse()
    {
        var command = new AceptarOfertaCommand
        {
            OfertaId = 1,
            AgenteId = "agente-unauthorized"
        };
        var oferta = new Oferta
        {
            Id = 1,
            PropiedadId = 5,
            Estado = EstadoOferta.Pendiente
        };
        var propiedad = new Propiedad
        {
            Id = 5,
            AgenteId = "agente-owner", 
            Estado = EstadoPropiedad.Disponible
        };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(command.OfertaId))
            .ReturnsAsync(oferta);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(oferta.PropiedadId))
            .ReturnsAsync(propiedad);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Mensaje.Should().Contain("permiso");
    }
    [Fact]
    public async Task Handle_When_OfertaAlreadyProcessed_Should_ReturnFailureResponse()
    {
        var command = new AceptarOfertaCommand
        {
            OfertaId = 1,
            AgenteId = "agente-123"
        };
        var oferta = new Oferta
        {
            Id = 1,
            PropiedadId = 5,
            Estado = EstadoOferta.Aceptada 
        };
        var propiedad = new Propiedad
        {
            Id = 5,
            AgenteId = "agente-123",
            Estado = EstadoPropiedad.Disponible
        };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(command.OfertaId))
            .ReturnsAsync(oferta);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(oferta.PropiedadId))
            .ReturnsAsync(propiedad);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Mensaje.Should().Contain("procesada");
    }
    [Fact]
    public async Task Handle_When_ValidCommand_Should_RejectOtherPendingOffers()
    {
        var command = new AceptarOfertaCommand
        {
            OfertaId = 1,
            AgenteId = "agente-123"
        };
        var ofertaAceptada = new Oferta
        {
            Id = 1,
            PropiedadId = 5,
            Estado = EstadoOferta.Pendiente
        };
        var ofertaPendiente = new Oferta
        {
            Id = 2,
            PropiedadId = 5,
            Estado = EstadoOferta.Pendiente
        };
        var propiedad = new Propiedad
        {
            Id = 5,
            AgenteId = "agente-123",
            Estado = EstadoPropiedad.Disponible
        };
        var todasOfertas = new List<Oferta> { ofertaAceptada, ofertaPendiente };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(command.OfertaId))
            .ReturnsAsync(ofertaAceptada);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(ofertaAceptada.PropiedadId))
            .ReturnsAsync(propiedad);
        _mockOfertaRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(todasOfertas);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockOfertaRepository.Verify(x => x.UpdateAsync(It.Is<Oferta>(o => 
            o.Id == 1 && 
            o.Estado == EstadoOferta.Aceptada
        )), Times.Once);
        _mockOfertaRepository.Verify(x => x.UpdateAsync(It.Is<Oferta>(o => 
            o.Id == 2 && 
            o.Estado == EstadoOferta.Rechazada
        )), Times.Once);
        _mockPropiedadRepository.Verify(x => x.UpdateAsync(It.Is<Propiedad>(p => 
            p.Id == 5 && 
            p.Estado == EstadoPropiedad.Vendida
        )), Times.Once);
    }
}