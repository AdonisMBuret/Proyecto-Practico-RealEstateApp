using System;
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

public class ChatRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetOrCreateChatAsync_CreatesNewChatWhenNotExists()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadAsync(context);
        var repository = new ChatRepository(context);

        var chat = await repository.GetOrCreateChatAsync(propiedad.Id, "cliente-1", "agente-1");

        chat.Should().NotBeNull();
        chat.PropiedadId.Should().Be(propiedad.Id);
        context.Chats.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOrCreateChatAsync_ReusesExistingChatRegardlessOfSenderOrder()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadAsync(context);
        var repository = new ChatRepository(context);

        var first = await repository.GetOrCreateChatAsync(propiedad.Id, "cliente-1", "agente-1");
        var second = await repository.GetOrCreateChatAsync(propiedad.Id, "agente-1", "cliente-1");

        second.Id.Should().Be(first.Id);
        context.Chats.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdWithMensajesAsync_ReturnsChatWithOrderedMessages()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadAsync(context);
        var chat = new Chat
        {
            PropiedadId = propiedad.Id,
            ClienteId = "cliente-1",
            AgenteId = "agente-1",
            FechaCreacion = DateTime.UtcNow.AddDays(-1)
        };
        context.Chats.Add(chat);
        await context.SaveChangesAsync();

        context.Mensajes.AddRange(
            new Mensaje
            {
                ChatId = chat.Id,
                Contenido = "Hola",
                FechaEnvio = DateTime.UtcNow.AddMinutes(-10),
                EmisorId = "cliente-1",
                ReceptorId = "agente-1",
                EsLeido = false
            },
            new Mensaje
            {
                ChatId = chat.Id,
                Contenido = "Respuesta",
                FechaEnvio = DateTime.UtcNow,
                EmisorId = "agente-1",
                ReceptorId = "cliente-1",
                EsLeido = true
            });
        await context.SaveChangesAsync();

        var repository = new ChatRepository(context);

        var result = await repository.GetByIdWithMensajesAsync(chat.Id);

        result.Should().NotBeNull();
        result!.Propiedad.Should().NotBeNull();
        result.Mensajes.Should().HaveCount(2);
        result.Mensajes.Should().BeInAscendingOrder(m => m.FechaEnvio);
    }

    [Fact]
    public async Task GetByPropiedadQueries_ReturnExpectedChats()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadAsync(context);
        context.Chats.AddRange(
            new Chat
            {
                PropiedadId = propiedad.Id,
                ClienteId = "cliente-1",
                AgenteId = "agente-1",
                FechaCreacion = DateTime.UtcNow.AddDays(-2)
            },
            new Chat
            {
                PropiedadId = propiedad.Id,
                ClienteId = "cliente-2",
                AgenteId = "agente-1",
                FechaCreacion = DateTime.UtcNow.AddDays(-1)
            },
            new Chat
            {
                PropiedadId = propiedad.Id + 1,
                ClienteId = "cliente-3",
                AgenteId = "agente-2",
                FechaCreacion = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        var repository = new ChatRepository(context);

        var chatCliente1 = await repository.GetByPropiedadAndClienteAsync(propiedad.Id, "cliente-1");
        var chatsPropiedad = await repository.GetByPropiedadIdAsync(propiedad.Id);

        chatCliente1.Should().NotBeNull();
        chatsPropiedad.Should().HaveCount(2);
        chatsPropiedad.All(c => c.PropiedadId == propiedad.Id).Should().BeTrue();
    }

    private static async Task<Propiedad> SeedPropiedadAsync(ApplicationDbContext context)
    {
        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = Guid.NewGuid().ToString()[..6],
            Precio = 150000m,
            TamanoEnMetros = 130,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            Descripcion = "Casa",
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-1"
        };

        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        return propiedad;
    }
}
