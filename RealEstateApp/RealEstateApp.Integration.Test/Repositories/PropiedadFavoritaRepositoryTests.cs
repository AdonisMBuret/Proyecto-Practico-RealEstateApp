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

public class PropiedadFavoritaRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task EsFavoritoAndGetByClienteYPropiedad_WorkAsExpected()
    {
        using var context = CreateContext();
        var dependencia = await SeedPropiedadAsync(context, "PROP100", DateTime.UtcNow.AddDays(-1));

        context.PropiedadesFavoritas.Add(new PropiedadFavorita
        {
            ClienteId = "cliente-1",
            PropiedadId = dependencia.Id
        });

        await context.SaveChangesAsync();

        var repository = new PropiedadFavoritaRepository(context);

        var esFavorito = await repository.EsFavoritoAsync("cliente-1", dependencia.Id);
        var esFavoritoOtro = await repository.EsFavoritoAsync("cliente-1", dependencia.Id + 1);
        var favorito = await repository.GetByClienteYPropiedadAsync("cliente-1", dependencia.Id);

        esFavorito.Should().BeTrue();
        esFavoritoOtro.Should().BeFalse();
        favorito.Should().NotBeNull();
        favorito!.ClienteId.Should().Be("cliente-1");
    }

    [Fact]
    public async Task GetPropiedadesFavoritas_ReturnsDetailedListOrderedByDate()
    {
        using var context = CreateContext();
        var primera = await SeedPropiedadAsync(context, "PROP200", DateTime.UtcNow.AddDays(-2));
        var segunda = await SeedPropiedadAsync(context, "PROP201", DateTime.UtcNow);

        context.PropiedadesFavoritas.AddRange(
            new PropiedadFavorita { ClienteId = "cliente-1", PropiedadId = primera.Id },
            new PropiedadFavorita { ClienteId = "cliente-1", PropiedadId = segunda.Id });

        await context.SaveChangesAsync();

        var repository = new PropiedadFavoritaRepository(context);

        var ids = await repository.GetPropiedadesFavoritasIdsAsync("cliente-1");
        var propiedades = await repository.GetPropiedadesFavoritasAsync("cliente-1");

        ids.Should().BeEquivalentTo(new List<int> { primera.Id, segunda.Id });
        propiedades.Should().HaveCount(2);
        propiedades.Should().BeInDescendingOrder(p => p.FechaCreacion);
        propiedades.All(p => p.TipoPropiedad != null && p.TipoVenta != null).Should().BeTrue();
        propiedades.SelectMany(p => p.PropiedadesMejoras).Should().NotBeEmpty();
        propiedades.SelectMany(p => p.Imagenes).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetCantidadFavoritosAsync_ReturnsCountForClient()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadAsync(context, "PROP300", DateTime.UtcNow);

        context.PropiedadesFavoritas.AddRange(
            new PropiedadFavorita { ClienteId = "cliente-1", PropiedadId = propiedad.Id },
            new PropiedadFavorita { ClienteId = "cliente-2", PropiedadId = propiedad.Id });

        await context.SaveChangesAsync();

        var repository = new PropiedadFavoritaRepository(context);

        var countCliente1 = await repository.GetCantidadFavoritosAsync("cliente-1");
        var countCliente3 = await repository.GetCantidadFavoritosAsync("cliente-3");

        countCliente1.Should().Be(1);
        countCliente3.Should().Be(0);
    }

    private static async Task<Propiedad> SeedPropiedadAsync(ApplicationDbContext context, string codigo, DateTime fecha)
    {
        var tipo = new TipoPropiedad { Nombre = $"Tipo-{codigo}", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = $"Venta-{codigo}", Descripcion = "Contado" };
        var mejora = new Mejora { Nombre = $"Mejora-{codigo}", Descripcion = "Extra" };

        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        context.Mejoras.Add(mejora);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = codigo,
            Precio = 120000m,
            TamanoEnMetros = 140,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            Descripcion = "Residencia",
            Estado = EstadoPropiedad.Disponible,
            FechaCreacion = fecha,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-1",
            Imagenes = new List<ImagenPropiedad>
            {
                new() { UrlImagen = $"{codigo}-img.jpg", EsPrincipal = true }
            }
        };

        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        context.PropiedadesMejoras.Add(new PropiedadMejora
        {
            PropiedadId = propiedad.Id,
            MejoraId = mejora.Id
        });

        await context.SaveChangesAsync();

        return propiedad;
    }
}
