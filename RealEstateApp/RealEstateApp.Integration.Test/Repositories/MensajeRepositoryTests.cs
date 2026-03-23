using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Persistence.Repositories;
using RealEstateApp.Integration.Test.Support;
using Xunit;

namespace RealEstateApp.Integration.Test.Repositories;

public class MensajeRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetMensajesByConversacionAsync_ReturnsOrderedMessages()
    {
        using var context = CreateContext();
        var chat = await SeedChatAsync(context);

        context.Mensajes.AddRange(
            new Mensaje
            {
                ChatId = chat.Id,
                Contenido = "Primer mensaje",
                FechaEnvio = DateTime.UtcNow.AddMinutes(-20),
                EmisorId = chat.ClienteId,
                ReceptorId = chat.AgenteId,
                EsLeido = false
            },
            new Mensaje
            {
                ChatId = chat.Id,
                Contenido = "Segundo mensaje",
                FechaEnvio = DateTime.UtcNow,
                EmisorId = chat.AgenteId,
                ReceptorId = chat.ClienteId,
                EsLeido = true
            });
        await context.SaveChangesAsync();

        var repository = new MensajeRepository(context);

        var mensajes = await repository.GetMensajesByConversacionAsync(chat.PropiedadId, chat.ClienteId, chat.AgenteId);

        mensajes.Should().HaveCount(2);
        mensajes.Should().BeInAscendingOrder(m => m.FechaEnvio);
    }

    [Fact]
    public async Task GetConversacionesByAgenteAsync_ReturnsChatsOrderedByLastMessage()
    {
        using var context = CreateContext();
        var chatA = await SeedChatAsync(context, "prop-1", DateTime.UtcNow.AddDays(-2));
        var chatB = await SeedChatAsync(context, "prop-2", DateTime.UtcNow.AddDays(-1));

        context.Mensajes.AddRange(
            new Mensaje
            {
                ChatId = chatA.Id,
                Contenido = "Chat A viejo",
                FechaEnvio = DateTime.UtcNow.AddHours(-10),
                EmisorId = chatA.ClienteId,
                ReceptorId = chatA.AgenteId,
                EsLeido = true
            },
            new Mensaje
            {
                ChatId = chatB.Id,
                Contenido = "Chat B reciente",
                FechaEnvio = DateTime.UtcNow,
                EmisorId = chatB.ClienteId,
                ReceptorId = chatB.AgenteId,
                EsLeido = false
            });
        await context.SaveChangesAsync();

        var repository = new MensajeRepository(context);

        var conversaciones = await repository.GetConversacionesByAgenteAsync("agente-1");

        conversaciones.Should().HaveCount(2);
        conversaciones.First().Id.Should().Be(chatB.Id);
        conversaciones.All(c => c.Propiedad != null && c.Mensajes.Any()).Should().BeTrue();
    }

    [Fact]
    public async Task GetCantidadMensajesNoLeidosAndMarcarComoLeido_WorkTogether()
    {
        using var context = CreateContext();
        var chat = await SeedChatAsync(context);
        var mensaje = new Mensaje
        {
            ChatId = chat.Id,
            Contenido = "Sin leer",
            FechaEnvio = DateTime.UtcNow,
            EmisorId = chat.ClienteId,
            ReceptorId = chat.AgenteId,
            EsLeido = false
        };
        context.Mensajes.AddRange(
            mensaje,
            new Mensaje
            {
                ChatId = chat.Id,
                Contenido = "Leído",
                FechaEnvio = DateTime.UtcNow.AddMinutes(-5),
                EmisorId = chat.AgenteId,
                ReceptorId = chat.ClienteId,
                EsLeido = true
            });
        await context.SaveChangesAsync();

        var repository = new MensajeRepository(context);

        var countAntes = await repository.GetCantidadMensajesNoLeidosAsync("agente-1");
        await repository.MarcarComoLeidoAsync(mensaje.Id);
        var countDespues = await repository.GetCantidadMensajesNoLeidosAsync("agente-1");

        countAntes.Should().Be(1);
        countDespues.Should().Be(0);
    }

    private static async Task<Chat> SeedChatAsync(ApplicationDbContext context, string codigoBase = "PROP", DateTime? fecha = null)
    {
        var tipo = new TipoPropiedad { Nombre = $"Tipo-{codigoBase}", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = $"Venta-{codigoBase}", Descripcion = "Contado" };
        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = codigoBase + Guid.NewGuid().ToString()[..3],
            Precio = 160000m,
            TamanoEnMetros = 140,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            Descripcion = "Casa",
            Estado = EstadoPropiedad.Disponible,
            FechaCreacion = fecha ?? DateTime.UtcNow,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-1"
        };
        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        var chat = new Chat
        {
            PropiedadId = propiedad.Id,
            ClienteId = "cliente-1",
            AgenteId = "agente-1",
            FechaCreacion = fecha ?? DateTime.UtcNow
        };
        context.Chats.Add(chat);
        await context.SaveChangesAsync();

        return chat;
    }
}
