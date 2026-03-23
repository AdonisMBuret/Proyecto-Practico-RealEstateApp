using System.Threading.Tasks;
using FluentAssertions;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Repositories;
using RealEstateApp.Integration.Test.Support;
using Xunit;

namespace RealEstateApp.Integration.Test.Repositories;

public class TipoVentaRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetByIdWithPropiedadesAsync_ReturnsVentaWithLoadedProperties()
    {
        using var context = CreateContext();
        var tipoVenta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        var tipoPropiedad = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };

        context.TiposVentas.Add(tipoVenta);
        context.TiposPropiedades.Add(tipoPropiedad);
        await context.SaveChangesAsync();

        context.Propiedades.AddRange(
            new Propiedad
            {
                Codigo = "TV001",
                Precio = 100000m,
                TamanoEnMetros = 120,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoPropiedad.Id,
                TipoVentaId = tipoVenta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "TV002",
                Precio = 130000m,
                TamanoEnMetros = 140,
                CantidadHabitaciones = 4,
                CantidadBanos = 3,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoPropiedad.Id,
                TipoVentaId = tipoVenta.Id,
                AgenteId = "agente-2"
            });

        await context.SaveChangesAsync();

        var repository = new TipoVentaRepository(context);

        var result = await repository.GetByIdWithPropiedadesAsync(tipoVenta.Id);

        result.Should().NotBeNull();
        result!.Propiedades.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExistsWithNameAsync_IsCaseInsensitiveAndHonorsExclusion()
    {
        using var context = CreateContext();
        var tipoVenta = new TipoVenta { Nombre = "Alquiler", Descripcion = "Mensual" };
        context.TiposVentas.Add(tipoVenta);
        await context.SaveChangesAsync();

        var repository = new TipoVentaRepository(context);

        var exists = await repository.ExistsWithNameAsync("ALQUILER");
        var excluded = await repository.ExistsWithNameAsync("Alquiler", tipoVenta.Id);

        exists.Should().BeTrue();
        excluded.Should().BeFalse();
    }

    [Fact]
    public async Task GetCantidadPropiedadesAsync_ReturnsCountAssociatedToVenta()
    {
        using var context = CreateContext();
        var tipoVenta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        var tipoRent = new TipoVenta { Nombre = "Alquiler", Descripcion = "Mensual" };
        var tipoPropiedad = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };

        context.TiposVentas.AddRange(tipoVenta, tipoRent);
        context.TiposPropiedades.Add(tipoPropiedad);
        await context.SaveChangesAsync();

        context.Propiedades.AddRange(
            new Propiedad
            {
                Codigo = "TV010",
                Precio = 100000m,
                TamanoEnMetros = 120,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoPropiedad.Id,
                TipoVentaId = tipoVenta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "TV011",
                Precio = 110000m,
                TamanoEnMetros = 110,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Casa",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoPropiedad.Id,
                TipoVentaId = tipoRent.Id,
                AgenteId = "agente-2"
            });

        await context.SaveChangesAsync();

        var repository = new TipoVentaRepository(context);

        var ventaCount = await repository.GetCantidadPropiedadesAsync(tipoVenta.Id);
        var rentCount = await repository.GetCantidadPropiedadesAsync(tipoRent.Id);

        ventaCount.Should().Be(1);
        rentCount.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdWithPropiedadesAsync_ReturnsNullWhenVentaDoesNotExist()
    {
        using var context = CreateContext();
        var repository = new TipoVentaRepository(context);

        var result = await repository.GetByIdWithPropiedadesAsync(404);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCantidadPropiedadesAsync_ReturnsZeroWhenNoAssignments()
    {
        using var context = CreateContext();
        var tipoVenta = new TipoVenta { Nombre = "Financiamiento", Descripcion = "Cuotas" };
        context.TiposVentas.Add(tipoVenta);
        await context.SaveChangesAsync();

        var repository = new TipoVentaRepository(context);

        var count = await repository.GetCantidadPropiedadesAsync(tipoVenta.Id);

        count.Should().Be(0);
    }
}
