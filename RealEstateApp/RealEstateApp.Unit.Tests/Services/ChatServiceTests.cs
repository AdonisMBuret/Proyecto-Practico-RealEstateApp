using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Services;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;
namespace RealEstateApp.Unit.Tests.Services;
public class ChatServiceTests
{
    private readonly Mock<IMensajeRepository> _mockMensajeRepository;
    private readonly Mock<IChatRepository> _mockChatRepository;
    private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ChatService>> _mockLogger;
    private readonly ChatService _chatService;
    public ChatServiceTests()
    {
        _mockMensajeRepository = new Mock<IMensajeRepository>();
        _mockChatRepository = new Mock<IChatRepository>();
        _mockUsuarioRepository = new Mock<IUsuarioRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ChatService>>();
        _chatService = new ChatService(
            _mockMensajeRepository.Object,
            _mockChatRepository.Object,
            _mockUsuarioRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }
    [Fact]
    public async Task EnviarMensajeAsync_When_ValidMessage_Should_CreateSuccessfully()
    {
        var viewModel = new SaveMensajeViewModel
        {
            PropiedadId = 1,
            EmisorId = "cliente-123",
            ReceptorId = "agente-456",
            Contenido = "¿Está disponible para visita?"
        };
        var chat = new Chat
        {
            Id = 1,
            PropiedadId = 1,
            ClienteId = "cliente-123",
            AgenteId = "agente-456"
        };
        var mensaje = new Mensaje
        {
            Id = 1,
            ChatId = 1,
            EmisorId = viewModel.EmisorId,
            Contenido = viewModel.Contenido,
            FechaEnvio = DateTime.UtcNow,
            EsLeido = false
        };
        var resultViewModel = new ChatViewModel
        {
            Id = mensaje.Id,
            Contenido = mensaje.Contenido,
            FechaEnvio = mensaje.FechaEnvio
        };
        _mockChatRepository
            .Setup(x => x.GetOrCreateChatAsync(viewModel.PropiedadId, viewModel.EmisorId, viewModel.ReceptorId))
            .ReturnsAsync(chat);
        _mockMapper
            .Setup(x => x.Map<Mensaje>(viewModel))
            .Returns(mensaje);
        _mockMensajeRepository
            .Setup(x => x.AddAsync(It.IsAny<Mensaje>()))
            .ReturnsAsync(mensaje);
        _mockMapper
            .Setup(x => x.Map<ChatViewModel>(mensaje))
            .Returns(resultViewModel);
        var result = await _chatService.EnviarMensajeAsync(viewModel);
        result.Should().NotBeNull();
        result.Contenido.Should().Be(viewModel.Contenido);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task EnviarMensajeAsync_When_EmptyContent_Should_ThrowArgumentException(string invalidContent)
    {
        var viewModel = new SaveMensajeViewModel
        {
            PropiedadId = 1,
            EmisorId = "cliente-123",
            ReceptorId = "agente-456",
            Contenido = invalidContent
        };
        await FluentActions
            .Invoking(() => _chatService.EnviarMensajeAsync(viewModel))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*contenido*");
    }
    [Fact]
    public async Task EnviarMensajeAsync_When_NullViewModel_Should_ThrowArgumentNullException()
    {
        await FluentActions
            .Invoking(() => _chatService.EnviarMensajeAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("viewModel");
    }
    [Fact]
    public async Task GetMensajesByConversacionAsync_Should_ReturnOrderedMessages()
    {
        var propiedadId = 1;
        var clienteId = "cliente-123";
        var agenteId = "agente-456";
        var mensajes = new List<Mensaje>
        {
            new() { Id = 1, Contenido = "Primer mensaje", FechaEnvio = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 2, Contenido = "Segundo mensaje", FechaEnvio = DateTime.UtcNow.AddHours(-1) }
        };
        var viewModels = mensajes.Select(m => new MensajeViewModel
        {
            Id = m.Id,
            Contenido = m.Contenido,
            FechaEnvio = m.FechaEnvio
        }).ToList();
        _mockMensajeRepository
            .Setup(x => x.GetMensajesByConversacionAsync(propiedadId, clienteId, agenteId))
            .ReturnsAsync(mensajes);
        _mockMapper
            .Setup(x => x.Map<List<MensajeViewModel>>(mensajes))
            .Returns(viewModels);
        var result = await _chatService.GetMensajesByConversacionAsync(propiedadId, clienteId, agenteId);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Contenido.Should().Be("Primer mensaje");
    }
    [Theory]
    [InlineData(0, "cliente-123", "agente-456")]     
    [InlineData(-1, "cliente-123", "agente-456")]    
    [InlineData(1, "", "agente-456")]                
    [InlineData(1, null, "agente-456")]              
    [InlineData(1, "cliente-123", "")]               
    [InlineData(1, "cliente-123", null)]             
    public async Task GetMensajesByConversacionAsync_When_InvalidParameters_Should_ReturnEmptyList(
        int propiedadId, string clienteId, string agenteId)
    {
        var result = await _chatService.GetMensajesByConversacionAsync(propiedadId, clienteId, agenteId);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    [Fact]
    public async Task GetConversacionesByAgenteAsync_Should_ReturnConversations()
    {
        var agenteId = "agente-123";
        var chats = new List<Chat>
        {
            new() { Id = 1, PropiedadId = 1, ClienteId = "cliente-1", AgenteId = agenteId },
            new() { Id = 2, PropiedadId = 2, ClienteId = "cliente-2", AgenteId = agenteId }
        };
        var conversacionViewModels = new List<ConversacionViewModel>
        {
            new()
            {
                PropiedadId = 1,
                CodigoPropiedad = "PROP001",
                ClienteId = "cliente-1",
                ClienteNombre = "", 
                UltimoMensaje = "Mensaje de prueba 1",
                FechaUltimoMensaje = DateTime.UtcNow.AddHours(-1),
                MensajesNoLeidos = 2,
                TotalMensajes = 5
            },
            new()
            {
                PropiedadId = 2,
                CodigoPropiedad = "PROP002",
                ClienteId = "cliente-2",
                ClienteNombre = "", 
                UltimoMensaje = "Mensaje de prueba 2",
                FechaUltimoMensaje = DateTime.UtcNow.AddHours(-2),
                MensajesNoLeidos = 1,
                TotalMensajes = 3
            }
        };
        _mockMensajeRepository
            .Setup(x => x.GetConversacionesByAgenteAsync(agenteId))
            .ReturnsAsync(chats);
        _mockMapper
            .Setup(x => x.Map<List<ConversacionViewModel>>(chats))
            .Returns(conversacionViewModels);
        _mockUsuarioRepository
            .Setup(x => x.GetUsuarioPerfilAsync("cliente-1"))
            .ReturnsAsync(("cliente-1", "Juan", "Pérez", "juan@test.com", "809-555-1234", null));
        _mockUsuarioRepository
            .Setup(x => x.GetUsuarioPerfilAsync("cliente-2"))
            .ReturnsAsync(("cliente-2", "María", "García", "maria@test.com", "809-555-5678", null));
        var result = await _chatService.GetConversacionesByAgenteAsync(agenteId);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(c => !string.IsNullOrEmpty(c.ClienteId)).Should().BeTrue();
        result.All(c => c.PropiedadId > 0).Should().BeTrue();
        result.First().ClienteNombre.Should().Be("Juan Pérez");
        result.First(). TieneMensajesNoLeidos.Should().BeTrue();
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetConversacionesByAgenteAsync_When_InvalidAgenteId_Should_ReturnEmptyList(string invalidAgenteId)
    {
        var result = await _chatService.GetConversacionesByAgenteAsync(invalidAgenteId);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
    [Fact]
    public async Task MarcarComoLeidoAsync_When_ValidId_Should_CallRepository()
    {
        var mensajeId = 1;
        _mockMensajeRepository
            .Setup(x => x.MarcarComoLeidoAsync(mensajeId))
            .Returns(Task.CompletedTask);
        await _chatService.MarcarComoLeidoAsync(mensajeId);
        _mockMensajeRepository.Verify(x => x.MarcarComoLeidoAsync(mensajeId), Times.Once);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task MarcarComoLeidoAsync_When_InvalidId_Should_ThrowArgumentException(int invalidId)
    {
        await FluentActions
            .Invoking(() => _chatService.MarcarComoLeidoAsync(invalidId))
            .Should().ThrowAsync<ArgumentException>()
            .WithParameterName("mensajeId");
    }
    [Fact]
    public async Task GetConversacionesByAgenteAsync_Should_ReturnConversationsWithCorrectProperties()
    {
        var agenteId = "agente-123";
        var chats = new List<Chat>
        {
            new() { Id = 1, PropiedadId = 1, ClienteId = "cliente-1", AgenteId = agenteId }
        };
        var conversacionViewModel = new ConversacionViewModel
        {
            PropiedadId = 1,
            CodigoPropiedad = "PROP001",
            ClienteId = "cliente-1",
            ClienteNombre = "", 
            UltimoMensaje = "Hola, estoy interesado en esta propiedad",
            FechaUltimoMensaje = DateTime.UtcNow,
            MensajesNoLeidos = 3,
            TotalMensajes = 8,
            EsConversacionActiva = true
        };
        _mockMensajeRepository
            .Setup(x => x.GetConversacionesByAgenteAsync(agenteId))
            .ReturnsAsync(chats);
        _mockMapper
            .Setup(x => x.Map<List<ConversacionViewModel>>(chats))
            .Returns(new List<ConversacionViewModel> { conversacionViewModel });
        _mockUsuarioRepository
            .Setup(x => x.GetUsuarioPerfilAsync("cliente-1"))
            .ReturnsAsync(("cliente-1", "Juan", "Pérez", "juan@test.com", "809-555-1234", null));
        var result = await _chatService.GetConversacionesByAgenteAsync(agenteId);
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var conversacion = result.First();
        conversacion.PropiedadId.Should().Be(1);
        conversacion.CodigoPropiedad.Should().Be("PROP001");
        conversacion.ClienteId.Should().Be("cliente-1");
        conversacion.ClienteNombre.Should().Be("Juan Pérez");
        conversacion.UltimoMensaje.Should().Be("Hola, estoy interesado en esta propiedad");
        conversacion.MensajesNoLeidos.Should().Be(3);
        conversacion.TotalMensajes.Should().Be(8);
        conversacion.TieneMensajesNoLeidos.Should().BeTrue();
        conversacion.EsConversacionActiva.Should().BeTrue();
    }
}