using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RealEstateApp.Application.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace RealEstateApp.Shared.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        _smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "";
        _smtpPass = _configuration["EmailSettings:SmtpPass"] ?? "";
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? _smtpUser;
        _fromName = _configuration["EmailSettings:FromName"] ?? "Real Estate App";
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
           
            if (string.IsNullOrEmpty(_smtpUser) || string.IsNullOrEmpty(_smtpPass))
            {
                _logger.LogWarning("Configuración de email incompleta. No se puede enviar email a {Email}", toEmail);
                return false;
            }

            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
            
            _logger.LogInformation("Email enviado exitosamente a {Email} con asunto: {Subject}", toEmail, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        var subject = "Confirma tu cuenta - Real Estate App";
        var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #8B4513; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 30px; background-color: #f9f9f9; }}
                    .button {{ display: inline-block; padding: 12px 30px; background-color: #D4AF37; color: #333; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                    .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>🏠 Real Estate App</h2>
                    </div>
                    <div class='content'>
                        <h3>¡Bienvenido a Real Estate App!</h3>
                        <p>Gracias por registrarte en nuestra plataforma de bienes raíces.</p>
                        <p>Para activar tu cuenta y comenzar a explorar propiedades, por favor confirma tu dirección de correo electrónico haciendo clic en el botón de abajo:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmationLink}' class='button'>Confirmar Mi Cuenta</a>
                        </p>
                        <p><strong>Nota:</strong> Si no creaste esta cuenta, puedes ignorar este correo de forma segura.</p>
                    </div>
                    <div class='footer'>
                        <p>Este es un correo automático, por favor no responder.</p>
                        <p>&copy; 2024 Real Estate App - República Dominicana</p>
                    </div>
                </div>
            </body>
            </html>
        ";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "¡Cuenta Activada - Bienvenido a Real Estate App! 🎉";
        
        var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #28A745; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 30px; background-color: #f9f9f9; }}
                    .features {{ background-color: white; padding: 20px; margin: 20px 0; border-left: 4px solid #D4AF37; }}
                    .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>🎉 ¡Cuenta Activada!</h2>
                    </div>
                    <div class='content'>
                        <h3>¡Hola {userName}!</h3>
                        <p>Tu cuenta ha sido <strong>activada exitosamente</strong>. ¡Ya puedes comenzar a explorar el mundo de los bienes raíces!</p>
                        
                        <div class='features'>
                            <h4>¿Qué puedes hacer ahora?</h4>
                            <ul>
                                <li>🔍 Explorar propiedades disponibles</li>
                                <li>❤️ Marcar propiedades como favoritas</li>
                                <li>💬 Contactar directamente con agentes</li>
                                <li>💰 Realizar ofertas por propiedades</li>
                                <li>📱 Acceder desde cualquier dispositivo</li>
                            </ul>
                        </div>
                        
                        <p style='text-align: center; margin: 30px 0; padding: 20px; background-color: #e8f5e8; border-radius: 5px;'>
                            <strong>Inicia sesión en Real Estate App para comenzar</strong>
                        </p>
                        
                        <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                        <p><strong>¡Bienvenido a la familia Real Estate App!</strong></p>
                    </div>
                    <div class='footer'>
                        <p>Real Estate App - Tu plataforma de confianza</p>
                        <p>&copy; 2024 Real Estate App - República Dominicana</p>
                    </div>
                </div>
            </body>
            </html>
        ";

        return await SendEmailAsync(toEmail, subject, body);
    }
}
