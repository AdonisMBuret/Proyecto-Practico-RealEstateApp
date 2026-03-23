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

public class OfertaRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetByClienteAsync_ReturnsOffersOrderedByDateWithPropertyIncluded()
    {
        using var context = CreateContext();
        var tipoPropiedad = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var tipoVenta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.Add(tipoPropiedad);
        context.TiposVentas.Add(tipoVenta);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "OF001",
            Precio = 150000m,
            TamanoEnMetros = 130,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            Descripcion = "Casa",
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = tipoPropiedad.Id,
            TipoVentaId = tipoVenta.Id,
            AgenteId = "agente-1"
        };

        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        context.Ofertas.AddRange(
            new Oferta
            {
                ClienteId = "cliente-1",
                PropiedadId = propiedad.Id,
                Monto = 140000m,
                Estado = EstadoOferta.Pendiente,
                FechaCreacion = DateTime.UtcNow.AddDays(-1)
            },
            new Oferta
            {
                ClienteId = "cliente-1",
                PropiedadId = propiedad.Id,
                Monto = 145000m,
                Estado = EstadoOferta.Pendiente,
                FechaCreacion = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var repository = new OfertaRepository(context);

        var result = await repository.GetByClienteAsync("cliente-1");

        result.Should().HaveCount(2);
        result.Should().BeInDescendingOrder(o => o.FechaCreacion);
        result.First().Propiedad.Should().NotBeNull();
    }

    [Fact]
    public async Task HasAcceptedOfertaAsync_ReturnsTrueWhenAcceptedOfferExists()
    {
        using var context = CreateContext();
        var tipoPropiedad = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var tipoVenta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.Add(tipoPropiedad);
        context.TiposVentas.Add(tipoVenta);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "OF010",
            Precio = 180000m,
            TamanoEnMetros = 150,
            CantidadHabitaciones = 4,
            CantidadBanos = 3,
            Descripcion = "Casa",
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = tipoPropiedad.Id,
            TipoVentaId = tipoVenta.Id,
            AgenteId = "agente-1"
        };

        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        context.Ofertas.AddRange(
            new Oferta
            {
                ClienteId = "cliente-1",
                PropiedadId = propiedad.Id,
                Monto = 170000m,
                Estado = EstadoOferta.Aceptada,
                FechaCreacion = DateTime.UtcNow
            },
            new Oferta
            {
                ClienteId = "cliente-2",
                PropiedadId = propiedad.Id,
                Monto = 175000m,
                Estado = EstadoOferta.Pendiente,
                FechaCreacion = DateTime.UtcNow.AddHours(-1)
            });

        await context.SaveChangesAsync();

        var repository = new OfertaRepository(context);

        var hasAccepted = await repository.HasAcceptedOfertaAsync(propiedad.Id);
        var otherProperty = await repository.HasAcceptedOfertaAsync(propiedad.Id + 1);

        hasAccepted.Should().BeTrue();
        otherProperty.Should().BeFalse();
    }
}
