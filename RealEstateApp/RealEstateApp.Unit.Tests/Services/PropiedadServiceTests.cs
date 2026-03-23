using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
namespace RealEstateApp.Unit.Tests.Services;
public class PropiedadServiceTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<PropiedadService>> _mockLogger;
    private readonly IPropiedadService _propiedadService;
    public PropiedadServiceTests()
    {
        _mockPropiedadRepository = new Mock<IPropiedadRepository>();
        _mockTipoPropiedadRepository = new Mock<ITipoPropiedadRepository>();
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<PropiedadService>>();
        _propiedadService = new PropiedadService(
            _mockPropiedadRepository.Object,
            _mockTipoPropiedadRepository.Object,
            _mockUsuarioRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }
    [Fact]
    public async Task GetAllDisponiblesAsync_ShouldReturnOnlyAvailableProperties()
    {
        var propiedades = new List<Propiedad>
        {
            new() { 
                Id = 1, 
                Codigo = "PROP001",
                Precio = 2_500_000m, 
                Estado = Domain.Enums.EstadoPropiedad.Disponible,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Id = 2, 
                Codigo = "PROP002",
                Precio = 4_200_000m, 
                Estado = Domain.Enums.EstadoPropiedad.Disponible,
                FechaCreacion = DateTime.UtcNow.AddHours(-1)
            }
        };
        var viewModels = propiedades.Select(p => new PropiedadViewModel
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Precio = p.Precio,
            FechaCreacion = p.FechaCreacion
        }).ToList();
        _mockPropiedadRepository
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(propiedades);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(propiedades))
            .Returns(viewModels);
        var result = await _propiedadService.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(p => p.Precio > 0).Should().BeTrue("Precios en DOP deben ser positivos");
        _mockPropiedadRepository.Verify(x => x.GetAllDisponiblesAsync(), Times.Once);
    }
    [Fact]
    public async Task GetByCodigoAsync_ExistingCode_ShouldReturnPropiedad()
    {
        var codigo = "PROP001";
        var propiedad = new Propiedad
        {
            Id = 1,
            Codigo = codigo,
            Precio = 3_800_000m, 
            Descripcion = "Casa en Santo Domingo"
        };
        var viewModel = new PropiedadViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            Precio = propiedad.Precio,
            Descripcion = propiedad.Descripcion
        };
        _mockPropiedadRepository
            .Setup(x => x.GetByCodigoAsync(codigo))
            .ReturnsAsync(propiedad);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedad))
            .Returns(viewModel);
        var result = await _propiedadService.GetByCodigoAsync(codigo);
        result.Should().NotBeNull();
        result!.Codigo.Should().Be(codigo);
        result.Precio.Should().Be(3_800_000m);
        _mockPropiedadRepository.Verify(x => x.GetByCodigoAsync(codigo), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_ValidPropiedad_ShouldCreateWithCorrectDOPPrice()
    {
        var agenteId = "agente-123";
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 5_200_000m, 
            Descripcion = "Apartamento en la Capital",
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            TamanoEnMetros = 95,
            MejorasSeleccionadas = new List<int> { 1, 2 }
        };
        var propiedad = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            TipoPropiedadId = viewModel.TipoPropiedadId,
            TipoVentaId = viewModel.TipoVentaId,
            Precio = viewModel.Precio,
            Descripcion = viewModel.Descripcion,
            AgenteId = agenteId,
            Estado = Domain.Enums.EstadoPropiedad.Disponible,
            FechaCreacion = DateTime.UtcNow
        };
        var resultViewModel = new PropiedadViewModel
        {
            Id = propiedad.Id,
            Codigo = propiedad.Codigo,
            Precio = propiedad.Precio
        };
        _mockPropiedadRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);
        _mockMapper
            .Setup(x => x.Map<Propiedad>(viewModel))
            .Returns(propiedad);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(propiedad);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedad))
            .Returns(resultViewModel);
        var result = await _propiedadService.CreateAsync(viewModel, agenteId);
        result.Should().NotBeNull();
        result.Precio.Should().Be(5_200_000m, "El precio debe mantenerse en DOP");
        result.Codigo.Should().NotBeNullOrEmpty("Debe generar código único");
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.Is<Propiedad>(p => 
            p.AgenteId == agenteId && 
            p.Precio == viewModel.Precio &&
            p.Estado == Domain.Enums.EstadoPropiedad.Disponible
        )), Times.Once);
    }
    [Fact]
    public async Task ExisteCodigoAsync_ExistingCode_ShouldReturnTrue()
    {
        var codigo = "PROP001";
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(codigo, null))
            .ReturnsAsync(true);
        var result = await _propiedadService.ExisteCodigoAsync(codigo);
        result.Should().BeTrue();
        _mockPropiedadRepository.Verify(x => x.ExisteCodigoAsync(codigo, null), Times.Once);
    }
    [Theory]
    [InlineData(1_000_000, 2_000_000)] 
    [InlineData(500_000, 1_500_000)]
    [InlineData(3_000_000, 10_000_000)]
    public async Task GetByFiltrosAsync_PriceRange_ShouldFilterByDOPPrices(decimal precioMin, decimal precioMax)
    {
        var filtros = new FiltrosPropiedadesViewModel
        {
            PrecioMinimo = precioMin,
            PrecioMaximo = precioMax
        };
        var propiedades = new List<Propiedad>
        {
            new() { Id = 1, Precio = 1_200_000m, FechaCreacion = DateTime.UtcNow }, 
            new() { Id = 2, Precio = 800_000m, FechaCreacion = DateTime.UtcNow.AddHours(-1) },   
            new() { Id = 3, Precio = 1_800_000m, FechaCreacion = DateTime.UtcNow.AddHours(-2) }, 
            new() { Id = 4, Precio = 2_500_000m, FechaCreacion = DateTime.UtcNow.AddHours(-3) }  
        };
        var propiedadesFiltradas = propiedades.Where(p => 
            p.Precio >= precioMin && p.Precio <= precioMax).ToList();
        var viewModels = propiedadesFiltradas.Select(p => new PropiedadViewModel
        {
            Id = p.Id,
            Precio = p.Precio,
            FechaCreacion = p.FechaCreacion
        }).ToList();
        _mockPropiedadRepository
            .Setup(x => x.GetByFiltrosAsync(
                filtros.TipoPropiedadId,
                filtros.PrecioMinimo,
                filtros.PrecioMaximo,
                filtros.CantidadHabitaciones,
                filtros.CantidadBanos))
            .ReturnsAsync(propiedadesFiltradas);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(propiedadesFiltradas))
            .Returns(viewModels);
        var result = await _propiedadService.GetByFiltrosAsync(filtros);
        result.Should().NotBeNull();
        result.All(p => p.Precio >= precioMin && p.Precio <= precioMax)
              .Should().BeTrue("Todas las propiedades deben estar en el rango de precios especificado");
    }
    [Fact]
    public async Task GetByCodigoAsync_NonExistentCode_ShouldReturnNull()
    {
        var codigo = "NONEXISTENT";
        _mockPropiedadRepository
            .Setup(x => x.GetByCodigoAsync(codigo))
            .ReturnsAsync((Propiedad?)null);
        var result = await _propiedadService.GetByCodigoAsync(codigo);
        result.Should().BeNull();
        _mockPropiedadRepository.Verify(x => x.GetByCodigoAsync(codigo), Times.Once);
    }
    [Fact]
    public async Task ExisteCodigoAsync_NonExistentCode_ShouldReturnFalse()
    {
        var codigo = "NONEXISTENT";
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(codigo, null))
            .ReturnsAsync(false);
        var result = await _propiedadService.ExisteCodigoAsync(codigo);
        result.Should().BeFalse();
        _mockPropiedadRepository.Verify(x => x.ExisteCodigoAsync(codigo, null), Times.Once);
    }
    [Fact]
    public async Task GetByAgenteIdAsync_ValidAgenteId_ShouldReturnProperties()
    {
        var agenteId = "agente-123";
        var propiedades = new List<Propiedad>
        {
            new() { 
                Id = 1, 
                AgenteId = agenteId, 
                Precio = 2_000_000m,
                FechaCreacion = DateTime.UtcNow
            },
            new() { 
                Id = 2, 
                AgenteId = agenteId, 
                Precio = 3_500_000m,
                FechaCreacion = DateTime.UtcNow.AddHours(-1)
            }
        };
        var viewModels = propiedades.Select(p => new PropiedadViewModel
        {
            Id = p.Id,
            AgenteId = p.AgenteId,
            Precio = p.Precio,
            FechaCreacion = p.FechaCreacion
        }).ToList();
        _mockPropiedadRepository
            .Setup(x => x.GetByAgenteIdAsync(agenteId, true))
            .ReturnsAsync(propiedades);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(propiedades))
            .Returns(viewModels);
        var result = await _propiedadService.GetByAgenteIdAsync(agenteId);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(p => p.AgenteId == agenteId).Should().BeTrue();
        _mockPropiedadRepository.Verify(x => x.GetByAgenteIdAsync(agenteId, true), Times.Once);
    }
}