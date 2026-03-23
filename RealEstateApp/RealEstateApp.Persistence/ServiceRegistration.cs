using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Application.Interfaces.Services;
using RealEstateApp.Domain.Interfaces;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Persistence.Repositories;
using RealEstateApp.Persistence.Services;

namespace RealEstateApp.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
                

        services.AddScoped(typeof(IRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        
       
        services.AddScoped<IPropiedadRepository, PropiedadRepository>();
        services.AddScoped<ITipoPropiedadRepository, TipoPropiedadRepository>();
        services.AddScoped<ITipoVentaRepository, TipoVentaRepository>();
        services.AddScoped<IMejoraRepository, MejoraRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IFavoritoRepository, PropiedadFavoritaRepository>(); 
        services.AddScoped<IMensajeRepository, MensajeRepository>(); 
        
        
        services.AddScoped<IOfertaRepository, OfertaRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        
        services.AddScoped<IImagenPropiedadService, ImagenPropiedadService>();
    }
}