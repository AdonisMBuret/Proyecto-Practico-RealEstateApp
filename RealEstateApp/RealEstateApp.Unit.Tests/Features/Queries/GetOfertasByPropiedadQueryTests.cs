using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Ofertas.Queries.GetOfertasByPropiedad;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Features.Queries;
public class GetOfertasByPropiedadQueryTests
{
    private readonly Mock<IOfertaRepository> _mockOfertaRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetOfertasByPropiedadQueryHandler _handler;
    public GetOfertasByPropiedadQueryTests()
    {
        _mockOfertaRepository = new Mock<IOfertaRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetOfertasByPropiedadQueryHandler(
            _mockOfertaRepository.Object,
            _mockMapper.Object);
    }
    [Fact]
    public async Task Handle_When_ValidQuery_Should_ReturnOfertasList()
    {
        var query = new GetOfertasByPropiedadQuery(1); 
        var allOfertas = new List<Oferta>
        {
            new()
            {
                Id = 1,
                PropiedadId = 1,
                ClienteId = "cliente-1",
                Monto = 2_000_000m,
                Estado = EstadoOferta.Pendiente,
                FechaCreacion = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = 2,
                PropiedadId = 1,
                ClienteId = "cliente-2",
                Monto = 2_200_000m,
                Estado = EstadoOferta.Aceptada,
                FechaCreacion = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = 3,
                PropiedadId = 2,
                ClienteId = "cliente-3",
                Monto = 1_800_000m,
                Estado = EstadoOferta.Pendiente,
                FechaCreacion = DateTime.UtcNow
            }
        };
        var expectedViewModels = new List<OfertaViewModel>
        {
            new()
            {
                Id = 2,
                PropiedadId = 1,
                ClienteId = "cliente-2",
                MontoOferta = 2_200_000m,
                EstadoTexto = "Aceptada"
            },
            new()
            {
                Id = 1,
                PropiedadId = 1,
                ClienteId = "cliente-1",
                MontoOferta = 2_000_000m,
                EstadoTexto = "Pendiente"
            }
        };
        _mockOfertaRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(allOfertas);
        _mockMapper
            .Setup(x => x.Map<List<OfertaViewModel>>(It.IsAny<List<Oferta>>()))
            .Returns(expectedViewModels);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].PropiedadId.Should().Be(1);
        result[0].MontoOferta.Should().Be(2_200_000m);
        result[0].EstadoTexto.Should().Be("Aceptada");
        result[1].MontoOferta.Should().Be(2_000_000m);
        result[1].EstadoTexto.Should().Be("Pendiente");
        _mockOfertaRepository.Verify(x => x.GetAllAsync(), Times.Once);
        _mockMapper.Verify(x => x.Map<List<OfertaViewModel>>(
            It.Is<List<Oferta>>(list => 
                list.Count == 2 && 
                list.All(o => o.PropiedadId == 1))), 
            Times.Once);
    }
    [Fact]
    public async Task Handle_When_NoOffersFound_Should_ReturnEmptyList()
    {
        var query = new GetOfertasByPropiedadQuery(999);
        var allOfertas = new List<Oferta>
        {
            new()
            {
                Id = 1,
                PropiedadId = 1,
                ClienteId = "cliente-1",
                Monto = 2_000_000m,
                Estado = EstadoOferta.Pendiente,
                FechaCreacion = DateTime.UtcNow
            }
        };
        _mockOfertaRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(allOfertas);
        _mockMapper
            .Setup(x => x.Map<List<OfertaViewModel>>(It.IsAny<List<Oferta>>()))
            .Returns(new List<OfertaViewModel>());
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockMapper.Verify(x => x.Map<List<OfertaViewModel>>(
            It.Is<List<Oferta>>(list => list.Count == 0)), 
            Times.Once);
    }
    [Fact]
    public async Task Handle_When_MultipleOffers_Should_ReturnOrderedByFechaCreacionDescending()
    {
        var query = new GetOfertasByPropiedadQuery(1);
        var allOfertas = new List<Oferta>
        {
            new()
            {
                Id = 1,
                PropiedadId = 1,
                FechaCreacion = DateTime.UtcNow.AddDays(-3), 
                Monto = 1_000_000m,
                Estado = EstadoOferta.Pendiente
            },
            new()
            {
                Id = 2,
                PropiedadId = 1,
                FechaCreacion = DateTime.UtcNow.AddDays(-1), 
                Monto = 2_000_000m,
                Estado = EstadoOferta.Aceptada
            },
            new()
            {
                Id = 3,
                PropiedadId = 1,
                FechaCreacion = DateTime.UtcNow.AddDays(-2), 
                Monto = 1_500_000m,
                Estado = EstadoOferta.Rechazada
            }
        };
        _mockOfertaRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(allOfertas);
        _mockMapper
            .Setup(x => x.Map<List<OfertaViewModel>>(It.IsAny<List<Oferta>>()))
            .Returns((List<Oferta> ofertas) => ofertas.Select(o => new OfertaViewModel 
            { 
                Id = o.Id, 
                MontoOferta = o.Monto 
            }).ToList());
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(2); 
        result[1].Id.Should().Be(3); 
        result[2].Id.Should().Be(1); 
        _mockMapper.Verify(x => x.Map<List<OfertaViewModel>>(
            It.Is<List<Oferta>>(list => 
                list.Count == 3 && 
                list[0].Id == 2 && 
                list[2].Id == 1)), 
            Times.Once);
    }
}