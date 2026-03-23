using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;
namespace RealEstateApp.Unit.Tests.Services;
public class PropiedadMejorasServiceTests
{
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<PropiedadService>> _mockLogger;
    private readonly PropiedadService _propiedadService;
    public PropiedadMejorasServiceTests()
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
            _mockLogger.Object);
    }
    #region CreateAsync - Mejoras Tests
    [Fact]
    public async Task CreateAsync_When_ValidPropertyWithMejoras_Should_SaveMejorasCorrectly()
    {
        var agenteId = "agente-123";
        var mejorasSeleccionadas = new List<int> { 1, 2, 3 }; 
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 5_000_000m,
            Descripcion = "Casa moderna con todas las comodidades",
            TamanoEnMetros = 250,
            CantidadHabitaciones = 4,
            CantidadBanos = 3,
            MejorasSeleccionadas = mejorasSeleccionadas
        };
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Casa" };
        var propiedadCreada = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            AgenteId = agenteId,
            Estado = EstadoPropiedad.Disponible
        };
        var propiedadViewModel = new PropiedadViewModel
        {
            Id = 1,
            Codigo = "PROP001"
        };
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(viewModel.TipoPropiedadId))
            .ReturnsAsync(tipoPropiedad);
        _mockPropiedadRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);
        _mockMapper
            .Setup(x => x.Map<Propiedad>(viewModel))
            .Returns(propiedadCreada);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(propiedadCreada);
        _mockPropiedadRepository
            .Setup(x => x.AddPropiedadMejoraAsync(It.IsAny<PropiedadMejora>()))
            .Returns(Task.CompletedTask);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedadCreada))
            .Returns(propiedadViewModel);
        var result = await _propiedadService.CreateAsync(viewModel, agenteId);
        result.Should().NotBeNull();
        result.Codigo.Should().Be("PROP001");
        _mockPropiedadRepository.Verify(
            x => x.AddPropiedadMejoraAsync(It.IsAny<PropiedadMejora>()),
            Times.Exactly(3));
    }
    [Fact]
    public async Task CreateAsync_When_PropertyWithNoMejoras_Should_NotCallAddPropiedadMejora()
    {
        var agenteId = "agente-123";
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 1_000_000m,
            Descripcion = "Terreno en excelente ubicación",
            TamanoEnMetros = 500,
            CantidadHabitaciones = 0,
            CantidadBanos = 0,
            MejorasSeleccionadas = new List<int>() 
        };
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Terreno" }; 
        var propiedadCreada = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            AgenteId = agenteId,
            Estado = EstadoPropiedad.Disponible
        };
        var propiedadViewModel = new PropiedadViewModel
        {
            Id = 1,
            Codigo = "PROP001"
        };
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(viewModel.TipoPropiedadId))
            .ReturnsAsync(tipoPropiedad);
        _mockPropiedadRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");
        _mockPropiedadRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);
        _mockMapper
            .Setup(x => x.Map<Propiedad>(viewModel))
            .Returns(propiedadCreada);
        _mockPropiedadRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(propiedadCreada);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedadCreada))
            .Returns(propiedadViewModel);
        var result = await _propiedadService.CreateAsync(viewModel, agenteId);
        result.Should().NotBeNull();
        _mockPropiedadRepository.Verify(
            x => x.AddPropiedadMejoraAsync(It.IsAny<PropiedadMejora>()),
            Times.Never);
    }
    [Fact]
    public async Task CreateAsync_When_CasaWithoutMejoras_Should_ThrowArgumentException()
    {
        var agenteId = "agente-123";
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 5_000_000m,
            Descripcion = "Casa sin mejoras seleccionadas",
            TamanoEnMetros = 200,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            MejorasSeleccionadas = null 
        };
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Casa" }; 
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(viewModel.TipoPropiedadId))
            .ReturnsAsync(tipoPropiedad);
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(viewModel, agenteId))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*mejora*");
    }
    #endregion
    #region UpdateAsync - Mejoras Tests
    [Fact]
    public async Task UpdateAsync_When_UpdateMejoras_Should_RemoveOldAndAddNew()
    {
        var agenteId = "agente-123";
        var propiedadId = 1;
        var nuevasMejoras = new List<int> { 4, 5 }; 
        var viewModel = new SavePropiedadViewModel
        {
            Id = propiedadId,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 6_000_000m,
            Descripcion = "Casa actualizada con nuevas mejoras",
            TamanoEnMetros = 250,
            CantidadHabitaciones = 4,
            CantidadBanos = 3,
            MejorasSeleccionadas = nuevasMejoras
        };
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Casa" };
        var propiedadExistente = new Propiedad
        {
            Id = propiedadId,
            Codigo = "PROP001",
            AgenteId = agenteId,
            Estado = EstadoPropiedad.Disponible
        };
        var propiedadViewModel = new PropiedadViewModel
        {
            Id = propiedadId,
            Codigo = "PROP001"
        };
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(propiedadId))
            .ReturnsAsync(propiedadExistente);
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(viewModel.TipoPropiedadId))
            .ReturnsAsync(tipoPropiedad);
        _mockMapper
            .Setup(x => x.Map(viewModel, propiedadExistente))
            .Returns(propiedadExistente);
        _mockPropiedadRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Propiedad>()))
            .Returns(Task.CompletedTask);
        _mockPropiedadRepository
            .Setup(x => x.RemovePropiedadMejorasAsync(propiedadId))
            .Returns(Task.CompletedTask);
        _mockPropiedadRepository
            .Setup(x => x.AddPropiedadMejoraAsync(It.IsAny<PropiedadMejora>()))
            .Returns(Task.CompletedTask);
        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedadExistente))
            .Returns(propiedadViewModel);
        var result = await _propiedadService.UpdateAsync(viewModel, agenteId);
        result.Should().NotBeNull();
        _mockPropiedadRepository.Verify(
            x => x.RemovePropiedadMejorasAsync(propiedadId),
            Times.Once);
        _mockPropiedadRepository.Verify(
            x => x.AddPropiedadMejoraAsync(It.IsAny<PropiedadMejora>()),
            Times.Exactly(2));
    }
    [Fact]
    public async Task UpdateAsync_When_PropertyNotBelongsToAgent_Should_ReturnNull()
    {
        var agenteId = "agente-123";
        var otroAgenteId = "agente-otro";
        var propiedadId = 1;
        var viewModel = new SavePropiedadViewModel
        {
            Id = propiedadId,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 5_000_000m,
            Descripcion = "Intento de actualización no autorizado",
            TamanoEnMetros = 200,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            MejorasSeleccionadas = new List<int> { 1, 2 }
        };
        var propiedadExistente = new Propiedad
        {
            Id = propiedadId,
            Codigo = "PROP001",
            AgenteId = otroAgenteId, 
            Estado = EstadoPropiedad.Disponible
        };
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(propiedadId))
            .ReturnsAsync(propiedadExistente);
        var result = await _propiedadService.UpdateAsync(viewModel, agenteId);
        result.Should().BeNull();
        _mockPropiedadRepository.Verify(
            x => x.RemovePropiedadMejorasAsync(It.IsAny<int>()),
            Times.Never);
    }
    #endregion
    #region GetPropiedadesByAgenteAsync Tests
    [Fact]
    public async Task GetPropiedadesByAgenteAsync_When_ValidAgenteId_Should_ReturnProperties()
    {
        var agenteId = "agente-123";
        var propiedades = new List<Propiedad>
        {
            new()
            {
                Id = 1,
                Codigo = "PROP001",
                AgenteId = agenteId,
                Estado = EstadoPropiedad.Disponible,
                PropiedadesMejoras = new List<PropiedadMejora>
                {
                    new() { MejoraId = 1, Mejora = new Mejora { Nombre = "Piscina" } },
                    new() { MejoraId = 2, Mejora = new Mejora { Nombre = "Gimnasio" } }
                }
            },
            new()
            {
                Id = 2,
                Codigo = "PROP002",
                AgenteId = agenteId,
                Estado = EstadoPropiedad.Disponible,
                PropiedadesMejoras = new List<PropiedadMejora>
                {
                    new() { MejoraId = 3, Mejora = new Mejora { Nombre = "Jardín" } }
                }
            }
        };
        var propiedadesViewModel = new List<PropiedadViewModel>
        {
            new() { Id = 1, Codigo = "PROP001", Mejoras = new List<string> { "Piscina", "Gimnasio" } },
            new() { Id = 2, Codigo = "PROP002", Mejoras = new List<string> { "Jardín" } }
        };
        _mockPropiedadRepository
            .Setup(x => x.GetByAgenteIdAsync(agenteId, true))
            .ReturnsAsync(propiedades);
        _mockMapper
            .Setup(x => x.Map<List<PropiedadViewModel>>(propiedades))
            .Returns(propiedadesViewModel);
        var result = await _propiedadService.GetPropiedadesByAgenteAsync(agenteId, incluirVendidas: false);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Mejoras.Should().HaveCount(2);
        result[0].Mejoras.Should().Contain("Piscina");
        result[0].Mejoras.Should().Contain("Gimnasio");
        result[1].Mejoras.Should().HaveCount(1);
        result[1].Mejoras.Should().Contain("Jardín");
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetPropiedadesByAgenteAsync_When_InvalidAgenteId_Should_ReturnEmptyList(string invalidAgenteId)
    {
        var result = await _propiedadService.GetPropiedadesByAgenteAsync(invalidAgenteId);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    #endregion
    #region Validation Tests
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_When_InvalidTipoPropiedadId_Should_ThrowArgumentException(int invalidId)
    {
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = invalidId,
            TipoVentaId = 1,
            Precio = 1_000_000m,
            Descripcion = "Descripción válida",
            TamanoEnMetros = 100,
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            MejorasSeleccionadas = new List<int> { 1 }
        };
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*tipo de propiedad*");
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_When_InvalidTipoVentaId_Should_ThrowArgumentException(int invalidId)
    {
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = invalidId,
            Precio = 1_000_000m,
            Descripcion = "Descripción válida",
            TamanoEnMetros = 100,
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            MejorasSeleccionadas = new List<int> { 1 }
        };
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*tipo de venta*");
    }
    [Theory]
    [InlineData(4999)] 
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_When_PriceBelowMinimum_Should_ThrowArgumentException(decimal invalidPrice)
    {
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = invalidPrice,
            Descripcion = "Descripción válida para la propiedad",
            TamanoEnMetros = 100,
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            MejorasSeleccionadas = new List<int> { 1 }
        };
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Casa" };
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(tipoPropiedad);
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*precio*");
    }
    [Fact]
    public async Task CreateAsync_When_DescriptionTooShort_Should_ThrowArgumentException()
    {
        var viewModel = new SavePropiedadViewModel
        {
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 1_000_000m,
            Descripcion = "Corta", 
            TamanoEnMetros = 100,
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            MejorasSeleccionadas = new List<int> { 1 }
        };
        var tipoPropiedad = new TipoPropiedad { Id = 1, Nombre = "Casa" };
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(tipoPropiedad);
        await FluentActions
            .Invoking(() => _propiedadService.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*descripción*10 caracteres*");
    }
    #endregion
}