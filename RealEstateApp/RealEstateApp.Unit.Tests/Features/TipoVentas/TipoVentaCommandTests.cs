using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.TipoVentas.Commands.CreateTipoVenta;
using RealEstateApp.Application.Features.TipoVentas.Commands.DeleteTipoVenta;
using RealEstateApp.Application.Features.TipoVentas.Commands.UpdateTipoVenta;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.TipoVentas;

public class TipoVentaCommandTests
{
    private readonly Mock<ITipoVentaRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly CreateTipoVentaCommandHandler _createHandler;
    private readonly UpdateTipoVentaCommandHandler _updateHandler;
    private readonly DeleteTipoVentaCommandHandler _deleteHandler;

    public TipoVentaCommandTests()
    {
        _repositoryMock = new Mock<ITipoVentaRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MantenimientoApiProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();

        _createHandler = new CreateTipoVentaCommandHandler(_repositoryMock.Object, _mapper);
        _updateHandler = new UpdateTipoVentaCommandHandler(_repositoryMock.Object, _mapper);
        _deleteHandler = new DeleteTipoVentaCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateTipoVenta_Should_ReturnDtoWithGeneratedId()
    {
        var command = new CreateTipoVentaCommand { Nombre = "Venta", Descripcion = "Contado" };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TipoVenta>()))
            .ReturnsAsync((TipoVenta t) =>
            {
                t.Id = 15;
                return t;
            });

        var result = await _createHandler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(15);
        result.Nombre.Should().Be(command.Nombre);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<TipoVenta>(t =>
            t.Nombre == command.Nombre && t.Descripcion == command.Descripcion)), Times.Once);
    }

    [Fact]
    public async Task CreateTipoVenta_Should_BubbleRepositoryErrors()
    {
        var command = new CreateTipoVentaCommand { Nombre = "Renta", Descripcion = "Mensual" };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TipoVenta>()))
            .ThrowsAsync(new InvalidOperationException("db fail"));

        var action = async () => await _createHandler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("db fail");
    }

    [Fact]
    public async Task UpdateTipoVenta_Should_ModifyOnlyNonEmptyFields()
    {
        var existing = new TipoVenta { Id = 9, Nombre = "Original", Descripcion = "Desc" };
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

        var command = new UpdateTipoVentaCommand
        {
            Id = existing.Id,
            Nombre = string.Empty,
            Descripcion = "Actualizado"
        };

        var result = await _updateHandler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(existing.Id);
        result.Nombre.Should().Be("Original");
        result.Descripcion.Should().Be("Actualizado");

        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<TipoVenta>(t =>
            t.Id == existing.Id && t.Nombre == "Original" && t.Descripcion == "Actualizado")), Times.Once);
    }

    [Fact]
    public async Task UpdateTipoVenta_Should_ThrowWhenEntityMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TipoVenta?)null);

        var command = new UpdateTipoVentaCommand { Id = 77, Nombre = "N/A" };

        var action = async () => await _updateHandler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tipo de venta con ID 77 no encontrado");
    }

    [Fact]
    public async Task DeleteTipoVenta_Should_DeleteWhenFound()
    {
        var existing = new TipoVenta { Id = 3, Nombre = "Eliminar" };
        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);

        await _deleteHandler.Handle(new DeleteTipoVentaCommand(existing.Id), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteAsync(existing), Times.Once);
    }

    [Fact]
    public async Task DeleteTipoVenta_Should_IgnoreMissingEntities()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TipoVenta?)null);

        await _deleteHandler.Handle(new DeleteTipoVentaCommand(111), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TipoVenta>()), Times.Never);
    }
}