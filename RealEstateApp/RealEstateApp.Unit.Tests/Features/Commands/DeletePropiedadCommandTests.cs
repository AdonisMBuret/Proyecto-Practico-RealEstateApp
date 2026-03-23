using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Propiedades.Commands.DeletePropiedad;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Commands;
public class DeletePropiedadCommandTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly DeletePropiedadCommandHandler _handler;
    public DeletePropiedadCommandTests()
    {
        _mockPropiedadRepository = new Mock<IPropiedadRepository>();
        _handler = new DeletePropiedadCommandHandler(
            _mockPropiedadRepository.Object
        );
    }
    [Fact]
    public async Task Handle_When_ValidId_Should_DeleteSuccessfully()
    {
        var command = new DeletePropiedadCommand(1);
        var existingPropiedad = CreateTestPropiedad();
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(existingPropiedad);
        _mockPropiedadRepository
            .Setup(x => x.DeleteAsync(existingPropiedad))
            .Returns(Task.CompletedTask);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Mensaje.Should().NotBeNullOrEmpty();
        result.Mensaje.ToLowerInvariant().Should().Contain("eliminada");
        _mockPropiedadRepository.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
        _mockPropiedadRepository.Verify(x => x.DeleteAsync(existingPropiedad), Times.Once);
    }
    [Fact]
    public async Task Handle_When_PropiedadNotFound_Should_ThrowKeyNotFoundException()
    {
        var command = new DeletePropiedadCommand(999);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync((Propiedad?)null);
        await FluentActions
            .Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
        _mockPropiedadRepository.Verify(x => x.DeleteAsync(It.IsAny<Propiedad>()), Times.Never);
    }
    [Fact]
    public async Task Handle_When_PropiedadExists_Should_DeleteRegardlessOfState()
    {
        var command = new DeletePropiedadCommand(1);
        var existingPropiedad = CreateTestPropiedad();
        existingPropiedad.Estado = EstadoPropiedad.Vendida; 
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(existingPropiedad);
        _mockPropiedadRepository
            .Setup(x => x.DeleteAsync(existingPropiedad))
            .Returns(Task.CompletedTask);
        var result = await _handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        _mockPropiedadRepository.Verify(x => x.DeleteAsync(existingPropiedad), Times.Once);
    }
    [Fact]
    public async Task Handle_When_RepositoryThrowsException_Should_PropagateException()
    {
        var command = new DeletePropiedadCommand(1);
        var existingPropiedad = CreateTestPropiedad();
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(command.Id))
            .ReturnsAsync(existingPropiedad);
        _mockPropiedadRepository
            .Setup(x => x.DeleteAsync(existingPropiedad))
            .ThrowsAsync(new InvalidOperationException("Database error"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    private static Propiedad CreateTestPropiedad() => new()
    {
        Id = 1,
        Codigo = "PROP001",
        Precio = 5_000_000m,
        Estado = EstadoPropiedad.Disponible,
        AgenteId = "agente-123"
    };
}