using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Interfaces;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Xunit;
namespace RealEstateApp.Unit.Tests.Services;
public class PropiedadServiceRefactoredTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<PropiedadService>> _mockLogger; 
    private readonly PropiedadService _propiedadService; 
    public PropiedadServiceRefactoredTests()
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
    #region GetAllDisponiblesAsync Tests
    [Fact]
    public async Task GetAllDisponiblesAsync_Should_ReturnOnlyAvailableProperties()
    {
        var propiedades = CreateTestPropiedades();
        var propiedadesDisponibles = propiedades.Where(p => p.Estado == EstadoPropiedad.Disponible).ToList();
        var viewModels = propiedadesDisponibles.Select(p => new PropiedadViewModel
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Precio = p.Precio,
            EstadoTexto = p.Estado.ToString() 
        }).ToList();
        _mockPropiedadRepository
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(propiedadesDisponibles);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(propiedadesDisponibles))
            .Returns(viewModels);
        var result = await _propiedadService.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(propiedadesDisponibles.Count);
        result.All(p => p.EstadoTexto == EstadoPropiedad.Disponible.ToString()).Should().BeTrue(); 
        result.All(p => p.Precio > 0).Should().BeTrue("Precios en DOP deben ser positivos");
        _mockPropiedadRepository.Verify(x => x.GetAllDisponiblesAsync(), Times.Once);
        _mockMapper.Verify(x => x.Map<List<PropiedadViewModel>>(propiedadesDisponibles), Times.Once);
    }
    [Fact]
    public async Task GetAllDisponiblesAsync_When_NoProperties_Should_ReturnEmptyList()
    {
        var emptyList = new List<Propiedad>();
        var emptyViewModels = new List<PropiedadViewModel>();
        _mockPropiedadRepository
            .Setup(x => x.GetAllDisponiblesAsync())
            .ReturnsAsync(emptyList);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(emptyList))
            .Returns(emptyViewModels);
        var result = await _propiedadService.GetAllDisponiblesAsync();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    #endregion
    #region GetByCodigoAsync Tests
    [Fact]
    public async Task GetByCodigoAsync_When_PropertyExists_Should_ReturnPropiedad()
    {
        var codigo = "PROP001";
        var propiedad = CreateTestPropiedad(1, codigo, 3_800_000m);
        var viewModel = CreateTestPropiedadViewModel(propiedad);
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
    }
    [Fact]
    public async Task GetByCodigoAsync_When_PropertyNotExists_Should_ReturnNull()
    {
        var codigo = "NONEXISTENT";
        _mockPropiedadRepository
            .Setup(x => x.GetByCodigoAsync(codigo))
            .ReturnsAsync((Propiedad?)null);
        var result = await _propiedadService.GetByCodigoAsync(codigo);
        result.Should().BeNull();
        _mockPropiedadRepository.Verify(x => x.GetByCodigoAsync(codigo), Times.Once);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetByCodigoAsync_When_InvalidCodigo_Should_ReturnNull(string invalidCodigo)
    {
        var result = await _propiedadService.GetByCodigoAsync(invalidCodigo);
        result.Should().BeNull();
        _mockPropiedadRepository.Verify(x => x.GetByCodigoAsync(It.IsAny<string>()), Times.Never);
    }
    #endregion
    #region CreateAsync Tests
    [Fact]
    public async Task CreateAsync_When_ValidPropiedad_Should_CreateSuccessfully()
    {
        var agenteId = "agente-123";
        var viewModel = CreateTestSavePropiedadViewModel();
        var propiedad = CreateTestPropiedad(0, "", viewModel.Precio);
        var createdPropiedad = CreateTestPropiedad(1, "PROP001", viewModel.Precio);
        var resultViewModel = CreateTestPropiedadViewModel(createdPropiedad);
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
            .ReturnsAsync(createdPropiedad);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(createdPropiedad))
            .Returns(resultViewModel);
        var result = await _propiedadService.CreateAsync(viewModel, agenteId);
        result.Should().NotBeNull();
        result.Precio.Should().Be(viewModel.Precio);
        result.Codigo.Should().NotBeNullOrEmpty();
        _mockPropiedadRepository.Verify(x => x.AddAsync(It.Is<Propiedad>(p => 
            p.AgenteId == agenteId && 
            p.Precio == viewModel.Precio &&
            p.Estado == EstadoPropiedad.Disponible
        )), Times.Once);
    }
    [Fact]
    public async Task CreateAsync_When_NullViewModel_Should_ThrowArgumentException()
    {
        var agenteId = "agente-123";
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(null!, agenteId))
            .Should().ThrowAsync<ArgumentNullException>();
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_When_InvalidAgenteId_Should_ThrowArgumentException(string invalidAgenteId)
    {
        var viewModel = CreateTestSavePropiedadViewModel();
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(viewModel, invalidAgenteId))
            .Should().ThrowAsync<ArgumentException>();
    }
    #endregion
    #region ExisteCodigoAsync Tests
    [Fact]
    public async Task ExisteCodigoAsync_When_CodigoExists_Should_ReturnTrue()
    {
        var codigo = "PROP001";
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(codigo, null))
            .ReturnsAsync(true);
        var result = await _propiedadService.ExisteCodigoAsync(codigo);
        result.Should().BeTrue();
        _mockPropiedadRepository.Verify(x => x.ExisteCodigoAsync(codigo, null), Times.Once);
    }
    [Fact]
    public async Task ExisteCodigoAsync_When_CodigoNotExists_Should_ReturnFalse()
    {
        var codigo = "NONEXISTENT";
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync(codigo, null))
            .ReturnsAsync(false);
        var result = await _propiedadService.ExisteCodigoAsync(codigo);
        result.Should().BeFalse();
    }
    #endregion
    #region GetByFiltrosAsync Tests
    [Theory]
    [InlineData(1_000_000, 2_000_000)]
    [InlineData(500_000, 1_500_000)]
    [InlineData(3_000_000, 10_000_000)]
    public async Task GetByFiltrosAsync_When_PriceFilter_Should_ReturnFilteredProperties(decimal precioMin, decimal precioMax)
    {
        var filtros = new FiltrosPropiedadesViewModel
        {
            PrecioMinimo = precioMin,
            PrecioMaximo = precioMax
        };
        var allPropiedades = CreateTestPropiedadesWithVariousPrices();
        var filteredPropiedades = allPropiedades.Where(p => 
            p.Precio >= precioMin && p.Precio <= precioMax).ToList();
        var viewModels = filteredPropiedades.Select(p => new PropiedadViewModel
        {
            Id = p.Id,
            Precio = p.Precio
        }).ToList();
        _mockPropiedadRepository
            .Setup(x => x.GetByFiltrosAsync(
                filtros.TipoPropiedadId,
                filtros.PrecioMinimo,
                filtros.PrecioMaximo,
                filtros.CantidadHabitaciones,
                filtros.CantidadBanos))
            .ReturnsAsync(filteredPropiedades);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(filteredPropiedades))
            .Returns(viewModels);
        var result = await _propiedadService.GetByFiltrosAsync(filtros);
        result.Should().NotBeNull();
        result.All(p => p.Precio >= precioMin && p.Precio <= precioMax)
              .Should().BeTrue("Todas las propiedades deben estar en el rango de precios");
    }
    #endregion
    #region Helper Methods
    private static List<Propiedad> CreateTestPropiedades() => new()
    {
        CreateTestPropiedad(1, "PROP001", 2_500_000m, EstadoPropiedad.Disponible),
        CreateTestPropiedad(2, "PROP002", 4_200_000m, EstadoPropiedad.Disponible),
        CreateTestPropiedad(3, "PROP003", 1_800_000m, EstadoPropiedad.Vendida) 
    };
    private static List<Propiedad> CreateTestPropiedadesWithVariousPrices() => new()
    {
        CreateTestPropiedad(1, "PROP001", 1_200_000m),
        CreateTestPropiedad(2, "PROP002", 800_000m),
        CreateTestPropiedad(3, "PROP003", 1_800_000m),
        CreateTestPropiedad(4, "PROP004", 2_500_000m)
    };
    private static Propiedad CreateTestPropiedad(int id, string codigo, decimal precio, EstadoPropiedad estado = EstadoPropiedad.Disponible) => new()
    {
        Id = id,
        Codigo = codigo,
        Precio = precio,
        Estado = estado,
        Descripcion = $"Propiedad de prueba {id}",
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        TamanoEnMetros = 95,
        AgenteId = "agente-123",
        FechaCreacion = DateTime.UtcNow,
        TipoPropiedadId = 1,
        TipoVentaId = 1
    };
    private static PropiedadViewModel CreateTestPropiedadViewModel(Propiedad propiedad) => new()
    {
        Id = propiedad.Id,
        Codigo = propiedad.Codigo,
        Precio = propiedad.Precio,
        Descripcion = propiedad.Descripcion,
        EstadoTexto = propiedad.Estado.ToString() 
    };
    private static SavePropiedadViewModel CreateTestSavePropiedadViewModel() => new()
    {
        TipoPropiedadId = 1,
        TipoVentaId = 1,
        Precio = 5_200_000m,
        Descripcion = "Apartamento en la Capital con más de diez caracteres",
        CantidadHabitaciones = 3,
        CantidadBanos = 2,
        TamanoEnMetros = 95,
        MejorasSeleccionadas = new List<int> { 1, 2 } 
    };
    #endregion
}