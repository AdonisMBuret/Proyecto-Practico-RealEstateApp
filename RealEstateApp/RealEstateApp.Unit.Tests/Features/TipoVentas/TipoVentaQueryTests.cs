using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.TipoVentas.Queries.GetAllTipoVentas;
using RealEstateApp.Application.Features.TipoVentas.Queries.GetTipoVentaById;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.TipoVentas;

public class TipoVentaQueryTests
{
    private readonly Mock<ITipoVentaRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllTipoVentasQueryHandler _getAllHandler;
    private readonly GetTipoVentaByIdQueryHandler _getByIdHandler;

    public TipoVentaQueryTests()
    {
        _repositoryMock = new Mock<ITipoVentaRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MantenimientoApiProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();

        _getAllHandler = new GetAllTipoVentasQueryHandler(_repositoryMock.Object, _mapper);
        _getByIdHandler = new GetTipoVentaByIdQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllTipoVentas_Should_ReturnCatalog()
    {
        var data = new List<TipoVenta>
        {
            new() { Id = 1, Nombre = "Venta", Descripcion = "Contado" },
            new() { Id = 2, Nombre = "Alquiler", Descripcion = "Mensual" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

        var result = await _getAllHandler.Handle(new GetAllTipoVentasQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(r => r.Nombre).Should().Contain(new[] { "Venta", "Alquiler" });
    }

    [Fact]
    public async Task GetAllTipoVentas_Should_ReturnEmptyList_When_NoData()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TipoVenta>());

        var result = await _getAllHandler.Handle(new GetAllTipoVentasQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTipoVentaById_Should_ReturnDtoWhenFound()
    {
        var entity = new TipoVenta { Id = 5, Nombre = "Financiamiento" };
        _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _getByIdHandler.Handle(new GetTipoVentaByIdQuery { Id = entity.Id }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
    }

    [Fact]
    public async Task GetTipoVentaById_Should_ReturnNullWhenMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TipoVenta?)null);

        var result = await _getByIdHandler.Handle(new GetTipoVentaByIdQuery { Id = 999 }, CancellationToken.None);

        result.Should().BeNull();
    }
}