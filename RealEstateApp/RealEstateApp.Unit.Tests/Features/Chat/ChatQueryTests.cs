using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using RealEstateApp.Application.Features.Chat.Queries.GetChatById;
using RealEstateApp.Application.Features.Chat.Queries.GetChatsByPropiedad;
using RealEstateApp.Application.Features.Chat.Queries.GetMensajesByChat;
using RealEstateApp.Application.Mappings.DtosAndViewModels;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Interfaces;
using ChatEntity = RealEstateApp.Domain.Entities.Chat;
using MensajeEntity = RealEstateApp.Domain.Entities.Mensaje;
using Xunit;

namespace RealEstateApp.Unit.Tests.Features.Chat;

public class ChatQueryTests
{
    private readonly Mock<IChatRepository> _chatRepositoryMock;
    private readonly Mock<IMensajeRepository> _mensajeRepositoryMock;
    private readonly IMapper _mapper;
    private readonly GetChatByIdQueryHandler _getChatByIdHandler;
    private readonly GetChatsByPropiedadQueryHandler _getChatsByPropiedadHandler;
    private readonly GetMensajesByChatQueryHandler _getMensajesByChatHandler;

    public ChatQueryTests()
    {
        _chatRepositoryMock = new Mock<IChatRepository>();
        _mensajeRepositoryMock = new Mock<IMensajeRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(ChatViewModelProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();

        _getChatByIdHandler = new GetChatByIdQueryHandler(_chatRepositoryMock.Object, _mapper);
        _getChatsByPropiedadHandler = new GetChatsByPropiedadQueryHandler(_chatRepositoryMock.Object, _mensajeRepositoryMock.Object, _mapper);
        _getMensajesByChatHandler = new GetMensajesByChatQueryHandler(_mensajeRepositoryMock.Object, _mapper);
    }

    

    [Fact]
    public async Task GetChatById_Should_ReturnNull_When_NotFound()
    {
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ChatEntity?)null);

        var result = await _getChatByIdHandler.Handle(new GetChatByIdQuery(999), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetChatsByPropiedad_Should_OrderByLatestMessageAndPopulateFallbacks()
    {
        var chats = new List<ChatEntity>
        {
            new() { Id = 1, PropiedadId = 10, ClienteId = "c1", AgenteId = "a1", FechaCreacion = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 2, PropiedadId = 10, ClienteId = "c2", AgenteId = "a2", FechaCreacion = DateTime.UtcNow.AddHours(-1) },
            new() { Id = 3, PropiedadId = 99, ClienteId = "c3", AgenteId = "a3", FechaCreacion = DateTime.UtcNow }
        };
        _chatRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(chats);

        var mensajes = new List<MensajeEntity>
        {
            new() { Id = 5, ChatId = 1, Contenido = "Hola", FechaEnvio = DateTime.UtcNow.AddMinutes(-30) },
            new() { Id = 6, ChatId = 2, Contenido = "Reciente", FechaEnvio = DateTime.UtcNow.AddMinutes(-5) }
        };
        _mensajeRepositoryMock.SetupSequence(r => r.GetAllAsync())
            .ReturnsAsync(mensajes)
            .ReturnsAsync(mensajes);

        var result = await _getChatsByPropiedadHandler.Handle(new GetChatsByPropiedadQuery(10), CancellationToken.None);

        result.Should().HaveCount(2);
        result.First().Contenido.Should().Be("Reciente");
        result.Last().Contenido.Should().Be("Hola");
    }

    [Fact]
    public async Task GetChatsByPropiedad_Should_UseFallbackMessage_When_NoMessages()
    {
        var chat = new ChatEntity { Id = 4, PropiedadId = 20, ClienteId = "c4", AgenteId = "a4", FechaCreacion = DateTime.UtcNow.AddMinutes(-10) };
        _chatRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ChatEntity> { chat });
        _mensajeRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<MensajeEntity>());

        var result = await _getChatsByPropiedadHandler.Handle(new GetChatsByPropiedadQuery(20), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Contenido.Should().Be("Sin mensajes");
        result[0].FechaEnvio.Should().BeCloseTo(chat.FechaCreacion, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetMensajesByChat_Should_ReturnOrderedViewModels()
    {
        var mensajes = new List<MensajeEntity>
        {
            new() { Id = 1, ChatId = 50, Contenido = "Primero", FechaEnvio = DateTime.UtcNow.AddMinutes(-5) },
            new() { Id = 2, ChatId = 50, Contenido = "Segundo", FechaEnvio = DateTime.UtcNow }
        };
        _mensajeRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(mensajes);

        var result = await _getMensajesByChatHandler.Handle(new GetMensajesByChatQuery(50), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Contenido.Should().Be("Primero");
        result[1].Contenido.Should().Be("Segundo");
    }
}