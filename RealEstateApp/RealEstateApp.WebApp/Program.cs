using Microsoft.AspNetCore.Identity;
using RealEstateApp.Identity.Entities;
using RealEstateApp.Persistence;
using RealEstateApp.Identity;
using RealEstateApp.Identity.Contexts;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Persistence.Seeds;
using RealEstateApp.Application;
using RealEstateApp.Shared;
using RealEstateApp.WebApp.Middleware;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace RealEstateApp.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Configure services
            builder.Services.AddApplicationLayer();
            builder.Services.AddPersistenceInfrastructure(builder.Configuration);
            builder.Services.AddIdentityInfrastructure(builder.Configuration);
            builder.Services.AddSharedServices();

            // Configure Problem Details
            builder.Services.AddProblemDetails();

            // Configurar localización para que los números decimales funcionen correctamente
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("es-DO"),
                    new CultureInfo("es-ES")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            
            var app = builder.Build();

            // Crear carpetas de imágenes si no existen
            var webRootPath = app.Environment.WebRootPath;
            if (!string.IsNullOrEmpty(webRootPath))
            {
                var imagesFolders = new[]
                {
                    Path.Combine(webRootPath, "images"),
                    Path.Combine(webRootPath, "images", "propiedades"),
                    Path.Combine(webRootPath, "images", "agentes"),
                    Path.Combine(webRootPath, "images", "usuarios")
                };

                foreach (var folder in imagesFolders)
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                }

                // Crear imagen placeholder si no existe
                var noImagePath = Path.Combine(webRootPath, "images", "no-image.png");
                if (!File.Exists(noImagePath))
                {
                    await CreatePlaceholderImageAsync(noImagePath);
                }
            }

            // Apply migrations and seed database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var identityContext = services.GetRequiredService<IdentityContext>();
                    await identityContext.Database.MigrateAsync();

                    var applicationContext = services.GetRequiredService<ApplicationDbContext>();
                    await applicationContext.Database.MigrateAsync();

                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await SeedDatabase.SeedAsync(context, userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error ejecutando los seeds de la base de datos");
                }
            }

            // Usar localización - IMPORTANTE: debe ir antes del middleware de rutas
            app.UseRequestLocalization();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseMiddleware<WebExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            await app.RunAsync();
        }

        private static async Task CreatePlaceholderImageAsync(string filePath)
        {
            try
            {
                var svgContent = """
                    <svg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300">
                        <rect width="400" height="300" fill="#f8f9fa" stroke="#dee2e6" stroke-width="2"/>
                        <text x="200" y="140" font-family="Arial, sans-serif" font-size="16" fill="#6c757d" text-anchor="middle">Sin imagen disponible</text>
                        <text x="200" y="160" font-family="Arial, sans-serif" font-size="12" fill="#6c757d" text-anchor="middle">Imagen no encontrada</text>
                        <circle cx="200" cy="100" r="30" fill="#e9ecef" stroke="#adb5bd"/>
                        <path d="M185 85 L215 85 L215 115 L185 115 Z M190 95 L210 95 L210 105 L190 105 Z" fill="#6c757d"/>
                    </svg>
                    """;
                
                await File.WriteAllTextAsync(filePath.Replace(".png", ".svg"), svgContent);
                
                var pngContent = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");
                await File.WriteAllBytesAsync(filePath, pngContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando imagen placeholder: {ex.Message}");
            }
        }
    }
}
