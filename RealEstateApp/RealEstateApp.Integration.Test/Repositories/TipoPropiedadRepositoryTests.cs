using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Repositories;
using RealEstateApp.Integration.Test.Support;
using Xunit;

namespace RealEstateApp.Integration.Test.Repositories;

public class TipoPropiedadRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetByIdWithPropiedadesAsync_ReturnsTypeWithLoadedProperties()
    {
        using var context = CreateContext();
        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        context.Propiedades.AddRange(
            new Propiedad
            {
                Codigo = "TP001",
                Precio = 100000m,
                TamanoEnMetros = 120,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipo.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "TP002",
                Precio = 130000m,
                TamanoEnMetros = 140,
                CantidadHabitaciones = 4,
                CantidadBanos = 3,
                Descripcion = "Casa grande",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipo.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            });

        await context.SaveChangesAsync();

        var repository = new TipoPropiedadRepository(context);

        var result = await repository.GetByIdWithPropiedadesAsync(tipo.Id);

        result.Should().NotBeNull();
        result!.Propiedades.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExisteConNombreAsync_IsCaseInsensitiveAndHonorsExclusion()
    {
        using var context = CreateContext();
        var tipo = new TipoPropiedad { Nombre = "Casa Lujo", Descripcion = "Residencial" };
        context.TiposPropiedades.Add(tipo);
        await context.SaveChangesAsync();

        var repository = new TipoPropiedadRepository(context);

        var exists = await repository.ExisteConNombreAsync("casa lujo");
        var excluded = await repository.ExisteConNombreAsync("casa lujo", tipo.Id);

        exists.Should().BeTrue();
        excluded.Should().BeFalse();
    }

    [Fact]
    public async Task GetCantidadPropiedadesAsync_ReturnsCorrectCount()
    {
        using var context = CreateContext();
        var tipoCasa = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var tipoApto = new TipoPropiedad { Nombre = "Apartamento", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.AddRange(tipoCasa, tipoApto);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        context.Propiedades.AddRange(
            new Propiedad
            {
                Codigo = "TP010",
                Precio = 100000m,
                TamanoEnMetros = 100,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoCasa.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "TP011",
                Precio = 125000m,
                TamanoEnMetros = 110,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoCasa.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-2"
            },
            new Propiedad
            {
                Codigo = "TP012",
                Precio = 90000m,
                TamanoEnMetros = 80,
                CantidadHabitaciones = 2,
                CantidadBanos = 1,
                Descripcion = "Apto",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoApto.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-3"
            });

        await context.SaveChangesAsync();

        var repository = new TipoPropiedadRepository(context);

        var countCasas = await repository.GetCantidadPropiedadesAsync(tipoCasa.Id);
        var countAptos = await repository.GetCantidadPropiedadesAsync(tipoApto.Id);

        countCasas.Should().Be(2);
        countAptos.Should().Be(1);
    }

    [Fact]
    public async Task GetTiposConPropiedadesAsync_ReturnsOrderedList()
    {
        using var context = CreateContext();
        var tipoZ = new TipoPropiedad { Nombre = "ZCasa", Descripcion = "Residencial" };
        var tipoA = new TipoPropiedad { Nombre = "Apartamento", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.AddRange(tipoZ, tipoA);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        context.Propiedades.AddRange(
            new Propiedad
            {
                Codigo = "TP100",
                Precio = 120000m,
                TamanoEnMetros = 115,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Z Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoZ.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "TP101",
                Precio = 95000m,
                TamanoEnMetros = 90,
                CantidadHabitaciones = 2,
                CantidadBanos = 1,
                Descripcion = "Apto",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoA.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-2"
            });
        await context.SaveChangesAsync();

        var repository = new TipoPropiedadRepository(context);

        var tipos = await repository.GetTiposConPropiedadesAsync();

        tipos.Should().HaveCount(2);
        tipos.Should().BeInAscendingOrder(tp => tp.Nombre);
        tipos.First().Propiedades.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdWithPropiedadesAsync_ReturnsNullWhenTypeDoesNotExist()
    {
        using var context = CreateContext();
        var repository = new TipoPropiedadRepository(context);

        var result = await repository.GetByIdWithPropiedadesAsync(555);

        result.Should().BeNull();
    }
}
