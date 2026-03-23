using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.TipoPropiedades.Commands.CreateTipoPropiedad;
using RealEstateApp.Application.Features.TipoPropiedades.Commands.DeleteTipoPropiedad;
using RealEstateApp.Application.Features.TipoPropiedades.Commands.UpdateTipoPropiedad;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.TipoPropiedades;
public class TipoPropiedadCommandTests
{
    private readonly Mock<ITipoPropiedadRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly CreateTipoPropiedadCommandHandler _createHandler;
    private readonly UpdateTipoPropiedadCommandHandler _updateHandler;
    private readonly DeleteTipoPropiedadCommandHandler _deleteHandler;
    public TipoPropiedadCommandTests()
    {
        _repositoryMock = new Mock<ITipoPropiedadRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MantenimientoApiProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();
        _createHandler = new CreateTipoPropiedadCommandHandler(_repositoryMock.Object, _mapper);
        _updateHandler = new UpdateTipoPropiedadCommandHandler(_repositoryMock.Object, _mapper);
        _deleteHandler = new DeleteTipoPropiedadCommandHandler(_repositoryMock.Object);
    }
    [Fact]
    public async Task CreateTipoPropiedad_Should_PersistAndReturnDto()
    {
        var command = new CreateTipoPropiedadCommand
        {
            Nombre = "Residencial",
            Descripcion = "Propiedades familiares"
        };
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TipoPropiedad>()))
            .ReturnsAsync((TipoPropiedad tp) =>
            {
                tp.Id = 10;
                return tp;
            });
        var result = await _createHandler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Nombre.Should().Be(command.Nombre);
        result.Descripcion.Should().Be(command.Descripcion);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<TipoPropiedad>(tp =>
            tp.Nombre == command.Nombre && tp.Descripcion == command.Descripcion)), Times.Once);
    }
    [Fact]
    public async Task CreateTipoPropiedad_Should_PropagateRepositoryExceptions()
    {
        var command = new CreateTipoPropiedadCommand
        {
            Nombre = "Industrial",
            Descripcion = "Naves"
        };
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TipoPropiedad>()))
            .ThrowsAsync(new InvalidOperationException("db error"));
        var action = async () => await _createHandler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("db error");
    }
    [Fact]
    public async Task UpdateTipoPropiedad_Should_UpdateOnlyProvidedFields()
    {
        var existing = new TipoPropiedad
        {
            Id = 7,
            Nombre = "Original",
            Descripcion = "Descripcion Original"
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
        var command = new UpdateTipoPropiedadCommand
        {
            Id = existing.Id,
            Nombre = "Actualizado",
            Descripcion = string.Empty 
        };
        var result = await _updateHandler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(existing.Id);
        result.Nombre.Should().Be("Actualizado");
        result.Descripcion.Should().Be("Descripcion Original");
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<TipoPropiedad>(tp =>
            tp.Id == existing.Id && tp.Nombre == "Actualizado" && tp.Descripcion == "Descripcion Original")), Times.Once);
    }
    [Fact]
    public async Task UpdateTipoPropiedad_Should_ThrowWhenEntityNotFound()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TipoPropiedad?)null);
        var command = new UpdateTipoPropiedadCommand
        {
            Id = 99,
            Nombre = "Inexistente",
            Descripcion = "No importa"
        };
        var action = async () => await _updateHandler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tipo de propiedad con ID 99 no encontrado");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TipoPropiedad>()), Times.Never);
    }
    [Fact]
    public async Task DeleteTipoPropiedad_Should_RemoveEntityWhenFound()
    {
        var existing = new TipoPropiedad { Id = 5, Nombre = "Temporal" };
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);
        await _deleteHandler.Handle(new DeleteTipoPropiedadCommand(existing.Id), CancellationToken.None);
        _repositoryMock.Verify(r => r.DeleteAsync(existing), Times.Once);
    }
    [Fact]
    public async Task DeleteTipoPropiedad_Should_BeIdempotentWhenEntityMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TipoPropiedad?)null);
        await _deleteHandler.Handle(new DeleteTipoPropiedadCommand(999), CancellationToken.None);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TipoPropiedad>()), Times.Never);
    }
}