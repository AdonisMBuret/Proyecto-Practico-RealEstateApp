using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Mejoras.Queries.GetAllMejoras;
using RealEstateApp.Application.Features.Mejoras.Queries.GetMejoraById;
using RealEstateApp.Application.Mappings;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Mejoras;

public class MejoraQueryTests
{
    private readonly Mock<IMejoraRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllMejorasQueryHandler _getAllHandler;
    private readonly GetMejoraByIdQueryHandler _getByIdHandler;

    public MejoraQueryTests()
    {
        _repositoryMock = new Mock<IMejoraRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(MantenimientoApiProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();

        _getAllHandler = new GetAllMejorasQueryHandler(_repositoryMock.Object, _mapper);
        _getByIdHandler = new GetMejoraByIdQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllMejoras_Should_ReturnMappedCatalog()
    {
        var mejoras = new List<Mejora>
        {
            new() { Id = 1, Nombre = "Piscina" },
            new() { Id = 2, Nombre = "Jardin" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(mejoras);

        var result = await _getAllHandler.Handle(new GetAllMejorasQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(r => r.Nombre).Should().Contain(new[] { "Piscina", "Jardin" });
    }

    [Fact]
    public async Task GetAllMejoras_Should_ReturnEmptyList_When_NoData()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Mejora>());

        var result = await _getAllHandler.Handle(new GetAllMejorasQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMejoraById_Should_ReturnDtoWhenFound()
    {
        var entity = new Mejora { Id = 10, Nombre = "Terraza" };
        _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);

        var result = await _getByIdHandler.Handle(new GetMejoraByIdQuery { Id = entity.Id }, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Nombre.Should().Be("Terraza");
    }

    [Fact]
    public async Task GetMejoraById_Should_ReturnNullWhenMissing()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Mejora?)null);

        var result = await _getByIdHandler.Handle(new GetMejoraByIdQuery { Id = 404 }, CancellationToken.None);

        result.Should().BeNull();
    }
}