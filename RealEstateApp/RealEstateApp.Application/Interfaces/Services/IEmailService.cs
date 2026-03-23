namespace RealEstateApp.Application.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    Task<bool> SendConfirmationEmailAsync(string toEmail, string confirmationLink);
    Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
}
