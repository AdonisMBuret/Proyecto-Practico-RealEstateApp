using FluentAssertions;
using Moq;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Xunit;

namespace RealEstateApp.Unit.Tests.Business;

public class PropiedadBusinessRulesTests
{
    private readonly Mock<IPropiedadRepository> _mockRepository;
    private readonly Mock<ITipoPropiedadRepository> _mockTipoPropiedadRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<PropiedadService>> _mockLogger;
    private readonly PropiedadService _service;

    public PropiedadBusinessRulesTests()
    {
        _mockRepository = new Mock<IPropiedadRepository>();
        _mockTipoPropiedadRepository = new Mock<ITipoPropiedadRepository>();
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<PropiedadService>>();
        _service = new PropiedadService(
            _mockRepository.Object, 
            _mockTipoPropiedadRepository.Object,
            _mockUsuarioRepository.Object,
            _mockMapper.Object, 
            _mockLogger.Object);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100_000)]
    public async Task CreateAsync_When_InvalidPrice_Should_ThrowArgumentException(decimal invalidPrice)
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = invalidPrice,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Descripcion = "Test description with more than 10 characters",
            CantidadHabitaciones = 1,
            CantidadBanos = 1,
            TamanoEnMetros = 50,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        await FluentActions
            .Invoking(() => _service.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*precio*"); 
    }

    [Theory]
    [InlineData(100)] 
    [InlineData(10_000_000_000)] 
    public async Task CreateAsync_When_UnrealisticPrice_Should_ThrowArgumentException(decimal unrealisticPrice)
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = unrealisticPrice,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Descripcion = "Test description with more than 10 characters",
            CantidadHabitaciones = 1,
            CantidadBanos = 1,
            TamanoEnMetros = 50,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        await FluentActions
            .Invoking(() => _service.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*rango*"); 
    }

    [Fact]
    public async Task CreateAsync_When_DuplicateCode_Should_ThrowInvalidOperationException()
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = 2_000_000m,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Descripcion = "Test description with more than 10 characters",
            CantidadHabitaciones = 1,
            CantidadBanos = 1,
            TamanoEnMetros = 50,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(true);

        
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Casa" });

        
        await FluentActions
            .Invoking(() => _service.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*código*único*"); 
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_When_InvalidRooms_ForCasa_Should_ThrowArgumentException(int invalidRooms)
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = 2_000_000m,
            TipoPropiedadId = 1, 
            TipoVentaId = 1,
            Descripcion = "Test description with more than 10 characters",
            CantidadHabitaciones = invalidRooms,
            CantidadBanos = 1,
            TamanoEnMetros = 50,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Casa" });

        
        await FluentActions
            .Invoking(() => _service.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*habitaciones*");
    }

    [Fact]
    public async Task CreateAsync_When_Terreno_WithZeroRooms_Should_CreateSuccessfully()
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = 2_000_000m,
            TipoPropiedadId = 4, 
            TipoVentaId = 1,
            Descripcion = "Terreno en zona residencial con más de 10 caracteres",
            CantidadHabitaciones = 0, 
            CantidadBanos = 0, 
            TamanoEnMetros = 500,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        var propiedadCreada = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            Precio = viewModel.Precio,
            AgenteId = "agente-123"
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(4))
            .ReturnsAsync(new TipoPropiedad { Id = 4, Nombre = "Terreno" });

        _mockMapper
            .Setup(x => x.Map<Propiedad>(viewModel))
            .Returns(new Propiedad { Precio = viewModel.Precio });

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(propiedadCreada);

        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedadCreada))
            .Returns(new PropiedadViewModel { Id = 1, Codigo = "PROP001" });

        
        var result = await _service.CreateAsync(viewModel, "agente-123");

        
        result.Should().NotBeNull();
        result.Codigo.Should().Be("PROP001");
    }

    [Fact]
    public async Task CreateAsync_When_LocalComercial_WithZeroRooms_Should_CreateSuccessfully()
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = 5_000_000m,
            TipoPropiedadId = 3, 
            TipoVentaId = 1,
            Descripcion = "Local comercial en zona céntrica con más de 10 caracteres",
            CantidadHabitaciones = 0, 
            CantidadBanos = 0, 
            TamanoEnMetros = 80,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        var propiedadCreada = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            Precio = viewModel.Precio,
            AgenteId = "agente-123"
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(3))
            .ReturnsAsync(new TipoPropiedad { Id = 3, Nombre = "Local Comercial" });

        _mockMapper
            .Setup(x => x.Map<Propiedad>(viewModel))
            .Returns(new Propiedad { Precio = viewModel.Precio });

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(propiedadCreada);

        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedadCreada))
            .Returns(new PropiedadViewModel { Id = 1, Codigo = "PROP001" });

        
        var result = await _service.CreateAsync(viewModel, "agente-123");

        
        result.Should().NotBeNull();
        result.Codigo.Should().Be("PROP001");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_When_InvalidSize_Should_ThrowArgumentException(int invalidSize)
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = 2_000_000m,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Descripcion = "Test description with more than 10 characters",
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            TamanoEnMetros = invalidSize,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Casa" });

        
        await FluentActions
            .Invoking(() => _service.CreateAsync(viewModel, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*tamaño*");
    }

    [Fact]
    public async Task DeleteAsync_When_PropertyNotBelongsToAgent_Should_ReturnFalse()
    {
        
        var propiedadId = 1;
        var agenteId = "agente-123";

        
        _mockRepository
            .Setup(x => x.GetByIdAsync(propiedadId))
            .ReturnsAsync((Propiedad?)null);

        
        var result = await _service.DeleteAsync(propiedadId, agenteId);

        
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_When_PropertyNotExists_Should_ReturnNull()
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Id = 1,
            Precio = 2_000_000m,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Descripcion = "Test description updated with more than 10 characters",
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            TamanoEnMetros = 50,
            MejorasSeleccionadas = new List<int> { 1 }
        };

        
        _mockRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Propiedad?)null);

        
        var result = await _service.UpdateAsync(viewModel, "agente-123");

        
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_When_ValidData_Should_CreateSuccessfully()
    {
        
        var viewModel = new SavePropiedadViewModel
        {
            Precio = 2_000_000m,
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Descripcion = "Valid test description with more than 10 characters",
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            TamanoEnMetros = 120,
            MejorasSeleccionadas = new List<int> { 1, 2 }
        };

        var propiedadCreada = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            Precio = viewModel.Precio,
            AgenteId = "agente-123"
        };

        
        _mockRepository
            .Setup(x => x.GenerarCodigoAsync())
            .ReturnsAsync("PROP001");

        _mockRepository
            .Setup(x => x.ExisteCodigoAsync("PROP001", null))
            .ReturnsAsync(false);

        
        _mockTipoPropiedadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new TipoPropiedad { Id = 1, Nombre = "Casa" });

        _mockMapper
            .Setup(x => x.Map<Propiedad>(viewModel))
            .Returns(new Propiedad { Precio = viewModel.Precio });

        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<Propiedad>()))
            .ReturnsAsync(propiedadCreada);

        _mockMapper
            .Setup(x => x.Map<PropiedadViewModel>(propiedadCreada))
            .Returns(new PropiedadViewModel { Id = 1, Codigo = "PROP001" });

        
        var result = await _service.CreateAsync(viewModel, "agente-123");

        
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Codigo.Should().Be("PROP001");
    }
}