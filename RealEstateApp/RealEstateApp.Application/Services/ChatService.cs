using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Application.ViewModels.Chat;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Services;

public class ChatService : IChatService
{
    private readonly IMensajeRepository _mensajeRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IMensajeRepository mensajeRepository,
        IChatRepository chatRepository,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<ChatService> logger)
    {
        _mensajeRepository = mensajeRepository;
        _chatRepository = chatRepository;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ChatViewModel> EnviarMensajeAsync(SaveMensajeViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        if (string.IsNullOrWhiteSpace(viewModel.Contenido))
            throw new ArgumentException("El contenido del mensaje no puede estar vacío", nameof(viewModel.Contenido));

        if (string.IsNullOrWhiteSpace(viewModel.EmisorId))
            throw new ArgumentException("El ID del emisor es requerido", nameof(viewModel.EmisorId));

        if (string.IsNullOrWhiteSpace(viewModel.ReceptorId))
            throw new ArgumentException("El ID del receptor es requerido", nameof(viewModel.ReceptorId));

        if (viewModel.PropiedadId <= 0)
            throw new ArgumentException("El ID de la propiedad debe ser mayor que cero", nameof(viewModel.PropiedadId));

        var chat = await _chatRepository.GetOrCreateChatAsync(viewModel.PropiedadId, viewModel.EmisorId, viewModel.ReceptorId);

        var mensaje = _mapper.Map<Mensaje>(viewModel);
        mensaje.ChatId = chat.Id;
        mensaje.FechaEnvio = DateTime.UtcNow;
        mensaje.EsLeido = false;

        var mensajeCreado = await _mensajeRepository.AddAsync(mensaje);
        
        var resultado = _mapper.Map<ChatViewModel>(mensajeCreado);
        
        _logger.LogInformation("Mensaje enviado de {EmisorId} a {ReceptorId} en propiedad {PropiedadId}", 
            viewModel.EmisorId, viewModel.ReceptorId, viewModel.PropiedadId);
        
        return resultado;
    }

    public async Task<List<ConversacionViewModel>> GetConversacionesByAgenteAsync(string agenteId)
    {
        if (string.IsNullOrWhiteSpace(agenteId))
            return new List<ConversacionViewModel>();

        var chats = await _mensajeRepository.GetConversacionesByAgenteAsync(agenteId);
        
        var conversaciones = _mapper.Map<List<ConversacionViewModel>>(chats);

        foreach (var conversacion in conversaciones)
        {
            var chat = chats.FirstOrDefault(c => c.ClienteId == conversacion.ClienteId);
            if (chat != null)
            {
                var clientePerfil = await _usuarioRepository.GetUsuarioPerfilAsync(chat.ClienteId);
                if (clientePerfil != default)
                {
                    conversacion.ClienteNombre = $"{clientePerfil.Nombre} {clientePerfil.Apellido}";
                }
                else
                {
                    conversacion.ClienteNombre = "Cliente no disponible";
                }
            }
        }
        
        return conversaciones;
    }

    public async Task<List<MensajeViewModel>> GetMensajesByConversacionAsync(int propiedadId, string clienteId, string agenteId)
    {
        if (propiedadId <= 0 || string.IsNullOrWhiteSpace(clienteId) || string.IsNullOrWhiteSpace(agenteId))
            return new List<MensajeViewModel>();

        var mensajes = await _mensajeRepository.GetMensajesByConversacionAsync(propiedadId, clienteId, agenteId);

        var mensajesViewModel = _mapper.Map<List<MensajeViewModel>>(mensajes);

        foreach (var mensajeViewModel in mensajesViewModel)
        {
            var mensajeOriginal = mensajes.FirstOrDefault(m => m.Id == mensajeViewModel.Id);
            if (mensajeOriginal != null)
            {
                var emisor = await _usuarioRepository.GetUsuarioPerfilAsync(mensajeOriginal.EmisorId);
                if (emisor != default)
                {
                    mensajeViewModel.EmisorNombre = $"{emisor.Nombre} {emisor.Apellido}";
                }
                
                var receptor = await _usuarioRepository.GetUsuarioPerfilAsync(mensajeOriginal.ReceptorId);
                if (receptor != default)
                {
                    mensajeViewModel.ReceptorNombre = $"{receptor.Nombre} {receptor.Apellido}";
                }
            }
        }

        await MarcarMensajesComoLeidosAsync(propiedadId, clienteId, agenteId);
        
        return mensajesViewModel;
    }

    private async Task MarcarMensajesComoLeidosAsync(int propiedadId, string clienteId, string agenteId)
    {
        var mensajes = await _mensajeRepository.GetMensajesByConversacionAsync(propiedadId, clienteId, agenteId);
        
        foreach (var mensaje in mensajes.Where(m => m.ReceptorId == agenteId && !m.EsLeido))
        {
            await _mensajeRepository.MarcarComoLeidoAsync(mensaje.Id);
        }
    }

    public async Task MarcarComoLeidoAsync(int mensajeId)
    {
        if (mensajeId <= 0)
            throw new ArgumentException("El ID del mensaje debe ser mayor que cero", nameof(mensajeId));

        await _mensajeRepository.MarcarComoLeidoAsync(mensajeId);
    }
}