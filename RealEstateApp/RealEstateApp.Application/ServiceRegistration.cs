using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RealEstateApp.Application.Behaviors;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Application.Services;
using System.Reflection;

namespace RealEstateApp.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        
        AddApplicationServices(services);

        return services;
    }

    private static void AddApplicationServices(IServiceCollection services)
    {

        services.AddScoped<IPropiedadService, PropiedadService>();
        services.AddScoped<IAgenteService, AgenteService>();


        services.AddScoped<ITipoPropiedadService, TipoPropiedadService>();
        services.AddScoped<ITipoVentaService, TipoVentaService>();
        services.AddScoped<IMejoraService, MejoraService>();


        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IOfertaService, OfertaService>();
        services.AddScoped<IFavoritoService, FavoritoService>();
        services.AddScoped<INotificacionService, NotificacionService>();

    }
}