using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Mejoras.Commands.CreateMejora;
using RealEstateApp.Application.Features.Mejoras.Commands.DeleteMejora;
using RealEstateApp.Application.Features.Mejoras.Commands.UpdateMejora;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Mejoras;

public class MejoraCommandTests
{
    private readonly Mock<IMejoraRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly CreateMejoraCommandHandler _createHandler;
    private readonly UpdateMejoraCommandHandler _updateHandler;
    private readonly DeleteMejoraCommandHandler _deleteHandler;

    public MejoraCommandTests()
    {
        _repositoryMock = new Mock<IMejoraRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MantenimientoApiProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();

        _createHandler = new CreateMejoraCommandHandler(_repositoryMock.Object, _mapper);
        _updateHandler = new UpdateMejoraCommandHandler(_repositoryMock.Object, _mapper);
        _deleteHandler = new DeleteMejoraCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateMejora_Should_ReturnMappedDto()
    {
        var command = new CreateMejoraCommand { Nombre = "Piscina", Descripcion = "Exterior" };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Mejora>()))
            .ReturnsAsync((Mejora m) =>
            {
                m.Id = 20;
                return m;
            });

        var result = await _createHandler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(20);
        result.Nombre.Should().Be("Piscina");

        _repositoryMock.Verify(r => r.AddAsync(It.Is<Mejora>(m =>
            m.Nombre == command.Nombre && m.Descripcion == command.Descripcion)), Times.Once);
    }

    [Fact]
    public async Task CreateMejora_Should_BubbleRepositoryErrors()
    {
        var command = new CreateMejoraCommand { Nombre = "Jardin", Descripcion = "Exterior" };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Mejora>()))
            .ThrowsAsync(new InvalidOperationException("failure"));

        var action = async () => await _createHandler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("failure");
    }

    [Fact]
    public async Task UpdateMejora_Should_UpdateOnlyProvidedFields()
    {
        var existing = new Mejora { Id = 5, Nombre = "Original", Descripcion = "Descripcion" };
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

        var command = new UpdateMejoraCommand
        {
            Id = existing.Id,
            Nombre = "Actualizada",
            Descripcion = string.Empty
        };

        var result = await _updateHandler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(existing.Id);
        result.Nombre.Should().Be("Actualizada");
        result.Descripcion.Should().Be("Descripcion");

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Mejora>(m =>
            m.Id == existing.Id && m.Nombre == "Actualizada" && m.Descripcion == "Descripcion")), Times.Once);
    }

    [Fact]
    public async Task UpdateMejora_Should_ThrowWhenEntityMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Mejora?)null);

        var command = new UpdateMejoraCommand { Id = 99, Nombre = "No existe" };

        var action = async () => await _updateHandler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Mejora con ID 99 no encontrada");
    }

    [Fact]
    public async Task DeleteMejora_Should_RemoveExisting()
    {
        var existing = new Mejora { Id = 2, Nombre = "Eliminar" };
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);

        await _deleteHandler.Handle(new DeleteMejoraCommand(existing.Id), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Fact]
    public async Task DeleteMejora_Should_BeIdempotentWhenMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Mejora?)null);

        await _deleteHandler.Handle(new DeleteMejoraCommand(123), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Mejora>()), Times.Never);
    }
}