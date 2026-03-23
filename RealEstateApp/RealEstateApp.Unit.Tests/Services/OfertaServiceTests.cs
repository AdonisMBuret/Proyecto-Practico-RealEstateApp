using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Ofertas;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;
namespace RealEstateApp.Unit.Tests.Services;
public class OfertaServiceTests
{
    private readonly Mock<IOfertaRepository> _mockOfertaRepository;
    private readonly Mock<IPropiedadRepository> _mockPropiedadRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository; 
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<OfertaService>> _mockLogger; 
    private readonly OfertaService _ofertaService;
    public OfertaServiceTests()
    {
        _mockOfertaRepository = new Mock<IOfertaRepository>();
        _mockPropiedadRepository = new Mock<IPropiedadRepository>();
        _mockUsuarioRepository = new Mock<IUsuarioRepository>(); 
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<OfertaService>>(); 
        _ofertaService = new OfertaService(
            _mockOfertaRepository.Object,
            _mockPropiedadRepository.Object,
            _mockUsuarioRepository.Object, 
            _mockMapper.Object,
            _mockLogger.Object); 
    }
    [Fact]
    public async Task CrearOfertaAsync_When_ValidOferta_Should_CreateSuccessfully() 
    {
        var viewModel = new SaveOfertaViewModel
        {
            PropiedadId = 1,
            ClienteId = "cliente-123",
            MontoOferta = 2_300_000m 
        };
        var oferta = new Oferta
        {
            Id = 1,
            PropiedadId = viewModel.PropiedadId,
            ClienteId = viewModel.ClienteId,
            Monto = viewModel.MontoOferta, 
            Estado = EstadoOferta.Pendiente
        };
        var resultViewModel = new OfertaViewModel
        {
            Id = oferta.Id,
            MontoOferta = oferta.Monto, 
            EstadoTexto = "Pendiente"
        };
        _mockPropiedadRepository
            .Setup(x => x.EstaDisponibleAsync(viewModel.PropiedadId))
            .ReturnsAsync(true);
        _mockOfertaRepository
            .Setup(x => x.TieneOfertasAceptadasAsync(viewModel.ClienteId, viewModel.PropiedadId)) 
            .ReturnsAsync(false);
        _mockUsuarioRepository
            .Setup(x => x.GetByIdAsync(viewModel.ClienteId))
            .ReturnsAsync(true); 
        _mockOfertaRepository
            .Setup(x => x.AddAsync(It.IsAny<Oferta>()))
            .ReturnsAsync(oferta);
        _mockMapper
            .Setup(x => x.Map<OfertaViewModel>(oferta))
            .Returns(resultViewModel);
        var result = await _ofertaService.CrearOfertaAsync(viewModel); 
        result.Should().NotBeNull();
        result.MontoOferta.Should().Be(2_300_000m);
        result.EstadoTexto.Should().Be("Pendiente");
    }
    [Fact]
    public async Task CrearOfertaAsync_When_PropertyNotAvailable_Should_ThrowInvalidOperationException()
    {
        var viewModel = new SaveOfertaViewModel
        {
            PropiedadId = 1,
            ClienteId = "cliente-123",
            MontoOferta = 2_000_000m
        };
        _mockUsuarioRepository
            .Setup(x => x.GetByIdAsync(viewModel.ClienteId))
            .ReturnsAsync(true);
        _mockPropiedadRepository
            .Setup(x => x.EstaDisponibleAsync(viewModel.PropiedadId))
            .ReturnsAsync(false);
        await FluentActions
            .Invoking(() => _ofertaService.CrearOfertaAsync(viewModel))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*no existe*no está disponible*");
    }
    [Fact]
    public async Task CrearOfertaAsync_When_ClienteNotExists_Should_ThrowInvalidOperationException()
    {
        var viewModel = new SaveOfertaViewModel
        {
            PropiedadId = 1,
            ClienteId = "cliente-inexistente",
            MontoOferta = 2_000_000m
        };
        _mockPropiedadRepository
            .Setup(x => x.EstaDisponibleAsync(viewModel.PropiedadId))
            .ReturnsAsync(true);
        _mockOfertaRepository
            .Setup(x => x.TieneOfertasAceptadasAsync(viewModel.ClienteId, viewModel.PropiedadId))
            .ReturnsAsync(false);
        _mockUsuarioRepository
            .Setup(x => x.GetByIdAsync(viewModel.ClienteId))
            .ReturnsAsync(false);
        await FluentActions
            .Invoking(() => _ofertaService.CrearOfertaAsync(viewModel))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cliente no encontrado*");
    }
    [Fact]
    public async Task CrearOfertaAsync_When_NullViewModel_Should_ThrowArgumentNullException()
    {
        await FluentActions
            .Invoking(() => _ofertaService.CrearOfertaAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("oferta");
    }
    [Fact]
    public async Task GetOfertasByClienteAsync_Should_ReturnClientOffers()
    {
        var clienteId = "cliente-123";
        var ofertas = new List<Oferta>
        {
            new() { Id = 1, ClienteId = clienteId, Monto = 2_000_000m, Estado = EstadoOferta.Pendiente },
            new() { Id = 2, ClienteId = clienteId, Monto = 2_500_000m, Estado = EstadoOferta.Aceptada }
        };
        var ofertasViewModel = ofertas.Select(o => new OfertaViewModel
        {
            Id = o.Id,
            ClienteId = o.ClienteId,
            MontoOferta = o.Monto,
            EstadoTexto = o.Estado.ToString()
        }).ToList();
        _mockOfertaRepository
            .Setup(x => x.GetByClienteAsync(clienteId))
            .ReturnsAsync(ofertas);
        _mockMapper
            .Setup(x => x.Map<List<OfertaViewModel>>(ofertas))
            .Returns(ofertasViewModel);
        var result = await _ofertaService.GetOfertasByClienteAsync(clienteId);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(o => o.ClienteId == clienteId).Should().BeTrue();
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetOfertasByClienteAsync_When_InvalidClienteId_Should_ReturnEmptyList(string invalidClienteId)
    {
        var result = await _ofertaService.GetOfertasByClienteAsync(invalidClienteId);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    [Fact]
    public async Task AceptarOfertaAsync_Should_AcceptOffer()
    {
        var ofertaId = 1;
        var agenteId = "agente-123";
        var oferta = new Oferta
        {
            Id = ofertaId,
            PropiedadId = 1,
            ClienteId = "cliente-123",
            Monto = 2_000_000m,
            Estado = EstadoOferta.Pendiente
        };
        var propiedad = new Propiedad
        {
            Id = 1,
            AgenteId = agenteId,
            Estado = EstadoPropiedad.Disponible
        };
        var ofertasPropiedad = new List<Oferta> { oferta };
        _mockOfertaRepository
            .Setup(x => x.GetByIdAsync(ofertaId))
            .ReturnsAsync(oferta);
        _mockPropiedadRepository
            .Setup(x => x.GetByIdAsync(oferta.PropiedadId))
            .ReturnsAsync(propiedad);
        _mockOfertaRepository
            .Setup(x => x.HasAcceptedOfertaAsync(oferta.PropiedadId))
            .ReturnsAsync(false);
        _mockOfertaRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Oferta>()))
            .Returns(Task.CompletedTask);
        _mockPropiedadRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Propiedad>()))
            .Returns(Task.CompletedTask);
        _mockOfertaRepository
            .Setup(x => x.GetByPropiedadAsync(oferta.PropiedadId))
            .ReturnsAsync(ofertasPropiedad);
        await _ofertaService.AceptarOfertaAsync(ofertaId, agenteId);
        _mockOfertaRepository.Verify(x => x.UpdateAsync(It.Is<Oferta>(o => 
            o.Id == ofertaId && o.Estado == EstadoOferta.Aceptada)), Times.Once);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AceptarOfertaAsync_When_InvalidOfertaId_Should_ThrowArgumentException(int invalidId)
    {
        await FluentActions
            .Invoking(() => _ofertaService.AceptarOfertaAsync(invalidId, "agente-123"))
            .Should().ThrowAsync<ArgumentException>()
            .WithParameterName("ofertaId");
    }
    [Fact]
    public async Task PuedeHacerOfertaAsync_When_ValidConditions_Should_ReturnTrue()
    {
        var clienteId = "cliente-123";
        var propiedadId = 1;
        _mockPropiedadRepository
            .Setup(x => x.EstaDisponibleAsync(propiedadId))
            .ReturnsAsync(true);
        _mockOfertaRepository
            .Setup(x => x.TieneOfertasAceptadasAsync(clienteId, propiedadId))
            .ReturnsAsync(false);
        var result = await _ofertaService.PuedeHacerOfertaAsync(clienteId, propiedadId);
        result.Should().BeTrue();
    }
    [Fact]
    public async Task PuedeHacerOfertaAsync_When_PropertyNotAvailable_Should_ReturnFalse()
    {
        var clienteId = "cliente-123";
        var propiedadId = 1;
        _mockPropiedadRepository
            .Setup(x => x.EstaDisponibleAsync(propiedadId))
            .ReturnsAsync(false);
        var result = await _ofertaService.PuedeHacerOfertaAsync(clienteId, propiedadId);
        result.Should().BeFalse();
    }
}