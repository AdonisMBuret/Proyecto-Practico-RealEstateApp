using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Repositories;
using RealEstateApp.Integration.Test.Support;
using Xunit;

namespace RealEstateApp.Integration.Test.Repositories;

public class MejoraRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetAllActiveAsync_ReturnsOrderedList()
    {
        using var context = CreateContext();
        context.Mejoras.AddRange(
            new Mejora { Nombre = "Balcón", Descripcion = "Exterior" },
            new Mejora { Nombre = "Ascensor", Descripcion = "Acceso" },
            new Mejora { Nombre = "Piscina", Descripcion = "Exterior" });

        await context.SaveChangesAsync();

        var repository = new MejoraRepository(context);

        var result = await repository.GetAllActiveAsync();

        result.Should().HaveCount(3);
        result.Select(m => m.Nombre).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetByIdsAndCantidadPropiedadesAsync_WorkAsExpected()
    {
        using var context = CreateContext();
        var mejoras = new List<Mejora>
        {
            new() { Nombre = "Piscina", Descripcion = "Exterior" },
            new() { Nombre = "Terraza", Descripcion = "Exterior" },
            new() { Nombre = "Jacuzzi", Descripcion = "Interior" }
        };
        context.Mejoras.AddRange(mejoras);

        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "MEJ001",
            Precio = 100000m,
            TamanoEnMetros = 120,
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

        context.PropiedadesMejoras.AddRange(
            new PropiedadMejora { PropiedadId = propiedad.Id, MejoraId = mejoras[0].Id },
            new PropiedadMejora { PropiedadId = propiedad.Id, MejoraId = mejoras[1].Id });

        await context.SaveChangesAsync();

        var repository = new MejoraRepository(context);

        var byIds = await repository.GetByIdsAsync(new List<int> { mejoras[0].Id, mejoras[2].Id });
        var count = await repository.GetCantidadPropiedadesAsync(mejoras[0].Id);
        var exists = await repository.ExistsWithNameAsync("piscina");
        var excluded = await repository.ExistsWithNameAsync("piscina", mejoras[0].Id);

        byIds.Should().HaveCount(2);
        count.Should().Be(1);
        exists.Should().BeTrue();
        excluded.Should().BeFalse();
    }
}
