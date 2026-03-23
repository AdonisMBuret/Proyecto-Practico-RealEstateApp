using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Agentes;
using RealEstateApp.Domain.Interfaces;
using Xunit;
namespace RealEstateApp.Unit.Tests.Services;
public class AgenteServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository; 
    private readonly Mock<IMapper> _mockMapper;
    private readonly AgenteService _agenteService;
    public AgenteServiceTests()
    {
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockPropiedadRepository = new Mock<IPropiedadRepository>(); 
        _mockMapper = new Mock<IMapper>();
        _agenteService = new AgenteService(
            _mockUsuarioRepository.Object, 
            _mockPropiedadRepository.Object, 
            _mockMapper.Object);
    }
    [Fact]
    public async Task GetAllActivosAsync_Should_ReturnAllActiveAgentes() 
    {
        var agentesIds = new List<string> { "agente-1", "agente-2" };
        _mockUsuarioRepository
            .Setup(x => x.GetAgenteActivosIdsAsync()) 
            .ReturnsAsync(agentesIds);
        _mockUsuarioRepository
            .Setup(x => x.GetAgentePerfilAsync("agente-1"))
            .ReturnsAsync(("agente-1", "Juan", "Pérez", "juan@test.com", "809-555-0123", "image1.jpg"));
        _mockUsuarioRepository
            .Setup(x => x.GetAgentePerfilAsync("agente-2"))
            .ReturnsAsync(("agente-2", "María", "García", "maria@test.com", "809-555-0124", "image2.jpg"));
        _mockPropiedadRepository
            .Setup(x => x.GetCantidadByAgenteAsync("agente-1"))
            .ReturnsAsync(3);
        _mockPropiedadRepository
            .Setup(x => x.GetCantidadByAgenteAsync("agente-2"))
            .ReturnsAsync(5);
        var result = await _agenteService.GetAllActivosAsync(); 
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(a => a.EsActivo).Should().BeTrue();
        result.Should().BeInAscendingOrder(a => a.NombreCompleto);
    }
    [Fact]
    public async Task GetByIdAsync_When_AgenteExists_Should_ReturnAgente()
    {
        var agenteId = "agente-123";
        _mockUsuarioRepository
            .Setup(x => x.ExisteAgenteAsync(agenteId))
            .ReturnsAsync(true);
        _mockUsuarioRepository
            .Setup(x => x.GetAgentePerfilAsync(agenteId))
            .ReturnsAsync(("agente-123", "Juan", "Pérez", "juan@test.com", "809-555-0123", "image.jpg"));
        var result = await _agenteService.GetByIdAsync(agenteId);
        result.Should().NotBeNull();
        result!.Id.Should().Be(agenteId);
        result.Nombre.Should().Be("Juan");
        result.Apellido.Should().Be("Pérez");
    }
    [Fact]
    public async Task GetByIdAsync_When_AgenteNotExists_Should_ReturnNull()
    {
        var agenteId = "agente-nonexistent";
        _mockUsuarioRepository
            .Setup(x => x.ExisteAgenteAsync(agenteId))
            .ReturnsAsync(false);
        var result = await _agenteService.GetByIdAsync(agenteId);
        result.Should().BeNull();
    }
    [Fact]
    public async Task ExisteAgenteAsync_When_ValidId_Should_ReturnTrue()
    {
        var agenteId = "agente-123";
        _mockUsuarioRepository
            .Setup(x => x.ExisteAgenteAsync(agenteId))
            .ReturnsAsync(true);
        var result = await _agenteService.ExisteAgenteAsync(agenteId);
        result.Should().BeTrue();
        _mockUsuarioRepository.Verify(x => x.ExisteAgenteAsync(agenteId), Times.Once);
    }
    [Fact]
    public async Task EsActivoAsync_When_ValidId_Should_ReturnActiveStatus()
    {
        var agenteId = "agente-123";
        _mockUsuarioRepository
            .Setup(x => x.EsAgenteActivoAsync(agenteId))
            .ReturnsAsync(true);
        var result = await _agenteService.EsActivoAsync(agenteId);
        result.Should().BeTrue();
        _mockUsuarioRepository.Verify(x => x.EsAgenteActivoAsync(agenteId), Times.Once);
    }
    [Fact]
    public async Task GetCantidadPropiedadesAsync_Should_ReturnCorrectCount()
    {
        var agenteId = "agente-123";
        var expectedCount = 5;
        _mockPropiedadRepository
            .Setup(x => x.GetCantidadByAgenteAsync(agenteId))
            .ReturnsAsync(expectedCount);
        var result = await _agenteService.GetCantidadPropiedadesAsync(agenteId);
        result.Should().Be(expectedCount);
    }
    [Fact]
    public async Task ActualizarPerfilAsync_When_ValidData_Should_ReturnTrue()
    {
        var agenteId = "agente-123";
        var viewModel = new EditarAgenteViewModel
        {
            Nombre = "Juan Carlos",
            Apellido = "Pérez López",
            Telefono = "809-555-9999",
            FotoActual = "updated-image.jpg"
        };
        _mockUsuarioRepository
            .Setup(x => x.ExisteAgenteAsync(agenteId))
            .ReturnsAsync(true);
        _mockUsuarioRepository
            .Setup(x => x.UpdateAgenteAsync(
                agenteId,
                viewModel.Nombre,
                viewModel.Apellido,
                viewModel.Telefono,
                viewModel.FotoActual))
            .ReturnsAsync(true);
        var result = await _agenteService.ActualizarPerfilAsync(agenteId, viewModel);
        result.Should().BeTrue();
    }
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task GetByIdAsync_When_InvalidId_Should_ReturnNull(string invalidId)
    {
        var result = await _agenteService.GetByIdAsync(invalidId);
        result.Should().BeNull();
    }
    [Fact]
    public async Task GetByNombreAsync_When_ValidName_Should_ReturnFilteredAgentes()
    {
        var nombre = "Juan";
        var agentesIds = new List<string> { "agente-1" };
        _mockUsuarioRepository
            .Setup(x => x.GetAgentesByNombreIdsAsync(nombre))
            .ReturnsAsync(agentesIds);
        _mockUsuarioRepository
            .Setup(x => x.GetAgentePerfilAsync("agente-1"))
            .ReturnsAsync(("agente-1", "Juan", "Pérez", "juan@test.com", "809-555-0123", "image.jpg"));
        _mockPropiedadRepository
            .Setup(x => x.GetCantidadByAgenteAsync("agente-1"))
            .ReturnsAsync(2);
        var result = await _agenteService.GetByNombreAsync(nombre);
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("Juan");
    }
    [Fact]
    public async Task GetPerfilAsync_When_ValidId_Should_ReturnProfile()
    {
        var agenteId = "agente-123";
        _mockUsuarioRepository
            .Setup(x => x.ExisteAgenteAsync(agenteId))
            .ReturnsAsync(true);
        _mockUsuarioRepository
            .Setup(x => x.GetAgentePerfilAsync(agenteId))
            .ReturnsAsync(("agente-123", "Juan", "Pérez", "juan@test.com", "809-555-0123", "image.jpg"));
        var result = await _agenteService.GetPerfilAsync(agenteId);
        result.Should().NotBeNull();
        result!.Id.Should().Be(agenteId);
        result.Nombre.Should().Be("Juan");
        result.Apellido.Should().Be("Pérez");
        result.Email.Should().Be("juan@test.com");
    }
}