using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Application;
using RealEstateApp.Persistence;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace RealEstateApp.Integration.Test;

public class DatabaseFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }
    public ApplicationDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var services = new ServiceCollection();
        
        // Configuración
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:"
            })
            .Build();

        // Registrar servicios
        services.AddSingleton<IConfiguration>(configuration);
        
        // Base de datos en memoria para pruebas de integración
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                   .EnableSensitiveDataLogging() // Para debugging
                   .EnableDetailedErrors());
        
        // Registrar capas de la aplicación
        services.AddApplicationLayer();
        services.AddPersistenceInfrastructure(configuration);

        ServiceProvider = services.BuildServiceProvider();
        
        // Crear un scope para obtener el contexto
        var scope = ServiceProvider.CreateScope(); // ? QUITAR using para evitar dispose temprano
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Asegurar que la base de datos esté creada
        Context.Database.EnsureCreated();
        
        // Seed completo para todas las entidades
        SeedComprehensiveTestData();
    }

    private void SeedComprehensiveTestData()
    {
        // Limpiar datos existentes si los hay - NOMBRES CORRECTOS
        if (Context.Ofertas.Any()) Context.RemoveRange(Context.Ofertas);
        if (Context.Mensajes.Any()) Context.RemoveRange(Context.Mensajes);
        if (Context.Chats.Any()) Context.RemoveRange(Context.Chats);
        if (Context.Propiedades.Any()) Context.RemoveRange(Context.Propiedades);
        if (Context.Mejoras.Any()) Context.RemoveRange(Context.Mejoras);
        if (Context.TiposVentas.Any()) Context.RemoveRange(Context.TiposVentas); 
        if (Context.TiposPropiedades.Any()) Context.RemoveRange(Context.TiposPropiedades); 
        Context.SaveChanges();

        // 1. Crear catálogos base
        var tipoPropiedad = new TipoPropiedad
        {
            Id = 1,
            Nombre = "Casa",
            Descripcion = "Casa familiar"
        };

        var tipoVenta = new TipoVenta
        {
            Id = 1,
            Nombre = "Venta",
            Descripcion = "Venta directa"
        };

        var mejora = new Mejora
        {
            Id = 1,
            Nombre = "Piscina",
            Descripcion = "Piscina privada"
        };

        Context.TiposPropiedades.Add(tipoPropiedad);
        Context.TiposVentas.Add(tipoVenta); 
        Context.Mejoras.Add(mejora);
        Context.SaveChanges();

        // 2. Crear propiedad de prueba
        var propiedad = new Propiedad
        {
            Id = 1,
            Codigo = "PROP001",
            TipoPropiedadId = 1,
            TipoVentaId = 1,
            Precio = 2_500_000m,
            Descripcion = "Casa de prueba",
            Estado = EstadoPropiedad.Disponible,
            AgenteId = "agente-test",
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            TamanoEnMetros = 120,
            FechaCreacion = DateTime.UtcNow.AddDays(-30)
        };

        Context.Propiedades.Add(propiedad);
        Context.SaveChanges();

        // 3. Crear chat de prueba
        var chat = new Chat
        {
            Id = 1,
            PropiedadId = 1,
            ClienteId = "cliente-test",
            AgenteId = "agente-test",
            FechaCreacion = DateTime.UtcNow.AddDays(-7)
        };

        Context.Chats.Add(chat);
        Context.SaveChanges();

        // 4. Crear mensaje de prueba
        var mensaje = new Mensaje
        {
            Id = 1,
            ChatId = 1,
            EmisorId = "cliente-test",
            Contenido = "Mensaje de prueba",
            FechaEnvio = DateTime.UtcNow.AddDays(-6)
        };

        Context.Mensajes.Add(mensaje);
        Context.SaveChanges();

        // 5. Crear oferta de prueba - PROPIEDADES CORRECTAS
        var oferta = new Oferta
        {
            Id = 1,
            PropiedadId = 1,
            ClienteId = "cliente-test",
            Monto = 2_300_000m, //
            Estado = EstadoOferta.Pendiente,
            FechaCreacion = DateTime.UtcNow.AddDays(-5)
            // ? REMOVIDO: Comentarios (no existe en la entidad)
        };

        Context.Ofertas.Add(oferta);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Database.EnsureDeleted();
        Context?.Dispose();
        
        if (ServiceProvider is IDisposable disposableProvider)
        {
            disposableProvider.Dispose();
        }
    }
}