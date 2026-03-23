namespace RealEstateApp.Application.Features.Chat.Commands.EnviarMensaje;

public class EnviarMensajeResponse
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int MensajeId { get; set; }
}
