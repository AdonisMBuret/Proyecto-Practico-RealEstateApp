using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Api.Handlers;

namespace RealEstateApp.Api.Extensions;


public static class ServiceCollectionExtensions
{
 
    public static IServiceCollection AddApiExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("O");
                context.ProblemDetails.Extensions["apiVersion"] = "1.0";
                
                var environment = context.HttpContext.RequestServices
                    .GetRequiredService<IWebHostEnvironment>();

               
                if (environment.IsDevelopment())
                {
                    context.ProblemDetails.Extensions["machine"] = Environment.MachineName;
                    context.ProblemDetails.Extensions["environment"] = environment.EnvironmentName;
                }
                else if (context.ProblemDetails.Status >= 500)
                {
                   
                    context.ProblemDetails.Detail = "Ha ocurrido un error interno. Contacte al administrador del sistema.";
                }
            };
        });

        
        services.AddExceptionHandler<GlobalExceptionHandler>();

        
        services.AddLogging(builder =>
        {
            builder.AddConsole()
                   .AddDebug()
                   .SetMinimumLevel(LogLevel.Information);
        });

        return services;
    }
}