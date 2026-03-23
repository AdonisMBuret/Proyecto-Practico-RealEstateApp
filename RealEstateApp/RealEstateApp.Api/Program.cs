using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RealEstateApp.Application;
using RealEstateApp.Identity;
using RealEstateApp.Identity.Entities;
using RealEstateApp.Identity.Seeds;
using RealEstateApp.Persistence;
using RealEstateApp.Shared;
using RealEstateApp.Api.Extensions;

namespace RealEstateApp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddPersistenceInfrastructure(builder.Configuration);
            
            builder.Services.AddIdentityInfrastructure(builder.Configuration, useJwtAsDefault: true);
            
            builder.Services.AddSharedServices();
            builder.Services.AddApplicationLayer(); 

            builder.Services.AddApiExceptionHandling();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RealEstateApp API",
                    Version = "v1.0",
                    Description = "API para la gestión de propiedades inmobiliarias - RFC 7807 Compliant",
                    Contact = new OpenApiContact
                    {
                        Name = "ITLA",
                        Email = "info@itla.edu.do"
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese 'Bearer' [espacio] y luego su token JWT.\r\n\r\nEjemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddProblemDetails();

             var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seedLogger = services.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                    
                    var adminExists = await roleManager.RoleExistsAsync("Administrador");
                    var devExists = await roleManager.RoleExistsAsync("Desarrollador");

                    if (adminExists || devExists)
                    {
                        seedLogger.LogInformation("Los roles por defecto ya existen en la base de datos. Se omite el seed.");
                    }
                    else
                    {
                        await DefaultRoles.SeedAsync(roleManager);
                        seedLogger.LogInformation("✅ Seed de roles completado");
                    }

                  
                    var adminUser = await userManager.FindByEmailAsync("admin@realestate.com");
                    var devUser = await userManager.FindByEmailAsync("dev@realestate.com");

                    if (adminUser != null && devUser != null)
                    {
                        seedLogger.LogInformation("Los usuarios por defecto ya existen en la base de datos. Se omite el seed de usuarios.");
                    }
                    else
                    {
                        await DefaultUsers.SeedAsync(userManager);
                        seedLogger.LogInformation("✅ Seed de usuarios completado");
                    }
                }
                catch (Exception ex)
                {
                    seedLogger.LogError(ex, "❌ Error al ejecutar el seed de roles y usuarios");
                }
            }

           
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstateApp API v1.0");
                    c.RoutePrefix = "swagger"; 
                    c.DocumentTitle = "RealEstateApp API - Documentación";
                    c.DisplayRequestDuration();
                });
            }

            app.UseApiExceptionHandling();

          
            app.UseRouting();

            app.UseCors("AllowAll");

            
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            
            app.MapControllers();

            
            var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
            appLogger.LogInformation("🚀 API configurada con autenticación JWT Bearer");
            
            try
            {
                var endpointSource = app.Services.GetRequiredService<EndpointDataSource>();
                foreach (var ep in endpointSource.Endpoints)
                {
                    appLogger.LogInformation("📍 Endpoint registrado: {Endpoint}", ep.DisplayName ?? ep.ToString());
                }
            }
            catch (Exception ex)
            {
                appLogger.LogWarning(ex, "No se pudieron listar endpoints");
            }

            await app.RunAsync();
        }
    }
}
