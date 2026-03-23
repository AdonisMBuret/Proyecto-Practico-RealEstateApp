using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Shared.Services;

namespace RealEstateApp.Shared;

public static class ServiceRegistration
{
    public static void AddSharedServices(this IServiceCollection services)
    {
        services.AddTransient<IFileUploadService, FileUploadService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IUserService, UserService>();
    }
}
