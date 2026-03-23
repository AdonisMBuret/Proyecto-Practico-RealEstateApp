using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.TipoPropiedades.Queries.GetAllTipoPropiedades;
using RealEstateApp.Application.Features.TipoPropiedades.Queries.GetTipoPropiedadById;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.TipoPropiedades;

public class TipoPropiedadQueryTests
{
    private readonly Mock<ITipoPropiedadRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllTipoPropiedadesQueryHandler _getAllHandler;
    private readonly GetTipoPropiedadByIdQueryHandler _getByIdHandler;

    public TipoPropiedadQueryTests()
    {
        _repositoryMock = new Mock<ITipoPropiedadRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MantenimientoApiProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();

        _getAllHandler = new GetAllTipoPropiedadesQueryHandler(_repositoryMock.Object, _mapper);
        _getByIdHandler = new GetTipoPropiedadByIdQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllTipoPropiedades_Should_ReturnMappedList()
    {
        var catalog = new List<TipoPropiedad>
        {
            new() { Id = 1, Nombre = "Casa", Descripcion = "Residencial" },
            new() { Id = 2, Nombre = "Apartamento", Descripcion = "Torre" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(catalog);

        var result = await _getAllHandler.Handle(new GetAllTipoPropiedadesQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(r => r.Nombre).Should().Contain(new[] { "Casa", "Apartamento" });
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllTipoPropiedades_Should_ReturnEmptyList_When_NoData()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TipoPropiedad>());

        var result = await _getAllHandler.Handle(new GetAllTipoPropiedadesQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTipoPropiedadById_Should_ReturnDtoWhenFound()
    {
        var entity = new TipoPropiedad { Id = 4, Nombre = "Comercial", Descripcion = "Locales" };
        _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _getByIdHandler.Handle(new GetTipoPropiedadByIdQuery { Id = entity.Id }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
        result.Nombre.Should().Be(entity.Nombre);
    }

    [Fact]
    public async Task GetTipoPropiedadById_Should_ReturnNullWhenMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TipoPropiedad?)null);

        var result = await _getByIdHandler.Handle(new GetTipoPropiedadByIdQuery { Id = 123 }, CancellationToken.None);

        result.Should().BeNull();
    }
}