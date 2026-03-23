using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RealEstateApp.Application.ViewModels.Propiedades;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Domain.Enums;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Persistence.Repositories;
using RealEstateApp.Integration.Test.Support;
using Xunit;

namespace RealEstateApp.Integration.Test.Repositories;

public class PropiedadRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task GetAllDisponiblesAsync_ReturnsOnlyAvailablePropertiesOrderedByDate()
    {
        using var context = CreateContext();
        var tipoCasa = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var tipoVenta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.Add(tipoCasa);
        context.TiposVentas.Add(tipoVenta);
        await context.SaveChangesAsync();

        var propiedades = new List<Propiedad>
        {
            new()
            {
                Codigo = "PROP001",
                Precio = 100000m,
                TamanoEnMetros = 120,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Disponible",
                Estado = EstadoPropiedad.Disponible,
                FechaCreacion = DateTime.UtcNow.AddDays(-1),
                TipoPropiedadId = tipoCasa.Id,
                TipoVentaId = tipoVenta.Id,
                AgenteId = "agente-1"
            },
            new()
            {
                Codigo = "PROP002",
                Precio = 150000m,
                TamanoEnMetros = 140,
                CantidadHabitaciones = 4,
                CantidadBanos = 3,
                Descripcion = "Vendida",
                Estado = EstadoPropiedad.Vendida,
                FechaCreacion = DateTime.UtcNow,
                TipoPropiedadId = tipoCasa.Id,
                TipoVentaId = tipoVenta.Id,
                AgenteId = "agente-1"
            },
            new()
            {
                Codigo = "PROP003",
                Precio = 90000m,
                TamanoEnMetros = 100,
                CantidadHabitaciones = 2,
                CantidadBanos = 1,
                Descripcion = "Reciente",
                Estado = EstadoPropiedad.Disponible,
                FechaCreacion = DateTime.UtcNow,
                TipoPropiedadId = tipoCasa.Id,
                TipoVentaId = tipoVenta.Id,
                AgenteId = "agente-2"
            }
        };

        context.Propiedades.AddRange(propiedades);
        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetAllDisponiblesAsync();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Estado == EstadoPropiedad.Disponible);
        result.Should().BeInDescendingOrder(p => p.FechaCreacion);
    }

    [Fact]
    public async Task DeleteAllByAgenteAsync_RemovesPropertiesAndDependents()
    {
        using var context = CreateContext();
        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        var mejora = new Mejora { Nombre = "Piscina", Descripcion = "Exterior" };

        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        context.Mejoras.Add(mejora);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "PROP010",
            Precio = 200000m,
            TamanoEnMetros = 180,
            CantidadHabitaciones = 4,
            CantidadBanos = 3,
            Descripcion = "Amplia",
            Estado = EstadoPropiedad.Disponible,
            FechaCreacion = DateTime.UtcNow,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-99"
        };

        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        context.PropiedadesMejoras.Add(new PropiedadMejora { PropiedadId = propiedad.Id, MejoraId = mejora.Id });
        context.PropiedadesFavoritas.Add(new PropiedadFavorita { PropiedadId = propiedad.Id, ClienteId = "cliente-1" });
        context.ImagenesPropiedades.Add(new ImagenPropiedad { PropiedadId = propiedad.Id, UrlImagen = "img-1.jpg", EsPrincipal = true });
        context.Ofertas.Add(new Oferta { PropiedadId = propiedad.Id, ClienteId = "cliente-1", Monto = 150000m, Estado = EstadoOferta.Pendiente, FechaCreacion = DateTime.UtcNow });

        var chat = new Chat
        {
            PropiedadId = propiedad.Id,
            ClienteId = "cliente-1",
            AgenteId = "agente-99",
            FechaCreacion = DateTime.UtcNow
        };

        context.Chats.Add(chat);
        await context.SaveChangesAsync();

        context.Mensajes.Add(new Mensaje
        {
            ChatId = chat.Id,
            Contenido = "Hola",
            FechaEnvio = DateTime.UtcNow,
            EmisorId = "cliente-1",
            ReceptorId = "agente-99",
            EsLeido = false
        });

        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        await repository.DeleteAllByAgenteAsync("agente-99");

        context.Propiedades.Should().BeEmpty();
        context.PropiedadesMejoras.Should().BeEmpty();
        context.PropiedadesFavoritas.Should().BeEmpty();
        context.ImagenesPropiedades.Should().BeEmpty();
        context.Ofertas.Should().BeEmpty();
        context.Chats.Should().BeEmpty();
        context.Mensajes.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerarCodigoAsync_ReturnsSequentialCode()
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
                Codigo = "PROP001",
                Precio = 100000m,
                TamanoEnMetros = 90,
                CantidadHabitaciones = 2,
                CantidadBanos = 1,
                Descripcion = "A",
                TipoPropiedadId = tipo.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "PROP010",
                Precio = 120000m,
                TamanoEnMetros = 110,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "B",
                TipoPropiedadId = tipo.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            });

        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        var codigo = await repository.GenerarCodigoAsync();

        codigo.Should().Be("PROP011");
    }

    [Fact]
    public async Task GetByFiltrosAsync_ReturnsPropertiesMatchingCombinedFilters()
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
                Codigo = "PROP201",
                Precio = 180000m,
                TamanoEnMetros = 150,
                CantidadHabitaciones = 4,
                CantidadBanos = 3,
                Descripcion = "Casa grande",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoCasa.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-1"
            },
            new Propiedad
            {
                Codigo = "PROP202",
                Precio = 90000m,
                TamanoEnMetros = 80,
                CantidadHabitaciones = 2,
                CantidadBanos = 1,
                Descripcion = "Apartamento",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipoApto.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-2"
            });

        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByFiltrosAsync(
            tipoPropiedadId: tipoCasa.Id,
            precioMin: 150000m,
            precioMax: 200000m,
            habitaciones: 4,
            banos: 2);

        result.Should().ContainSingle();
        result.First().Codigo.Should().Be("PROP201");
    }

    [Fact]
    public async Task GetByCodigoAsync_ReturnsPropertyWithDetails()
    {
        using var context = CreateContext();
        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        var mejora = new Mejora { Nombre = "Piscina", Descripcion = "Exterior" };

        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        context.Mejoras.Add(mejora);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "COD101",
            Precio = 220000m,
            TamanoEnMetros = 200,
            CantidadHabitaciones = 5,
            CantidadBanos = 3,
            Descripcion = "Residencia premium",
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-1",
            Imagenes =
            {
                new ImagenPropiedad { UrlImagen = "cod101-1.jpg", EsPrincipal = true }
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

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByCodigoAsync("COD101");

        result.Should().NotBeNull();
        result!.TipoPropiedad.Should().NotBeNull();
        result.TipoVenta.Should().NotBeNull();
        result.Imagenes.Should().ContainSingle();
        result.PropiedadesMejoras.Should().ContainSingle(pm => pm.Mejora.Nombre == "Piscina");
    }

    [Fact]
    public async Task GetByAgenteIdAsync_RespectsAvailabilityFilter()
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
                Codigo = "AG001",
                Precio = 120000m,
                TamanoEnMetros = 110,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Disponible",
                Estado = EstadoPropiedad.Disponible,
                TipoPropiedadId = tipo.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-77"
            },
            new Propiedad
            {
                Codigo = "AG002",
                Precio = 130000m,
                TamanoEnMetros = 115,
                CantidadHabitaciones = 3,
                CantidadBanos = 2,
                Descripcion = "Vendida",
                Estado = EstadoPropiedad.Vendida,
                TipoPropiedadId = tipo.Id,
                TipoVentaId = venta.Id,
                AgenteId = "agente-77"
            });
        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        var soloDisponibles = await repository.GetByAgenteIdAsync("agente-77");
        var todas = await repository.GetByAgenteIdAsync("agente-77", soloDisponibles: false);

        soloDisponibles.Should().ContainSingle(p => p.Codigo == "AG001");
        todas.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExisteCodigoAsyncAndEstaDisponibleAsync_ReturnExpectedFlags()
    {
        using var context = CreateContext();
        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };

        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "CHECK01",
            Precio = 100000m,
            TamanoEnMetros = 90,
            CantidadHabitaciones = 2,
            CantidadBanos = 1,
            Descripcion = "Chequear",
            Estado = EstadoPropiedad.Disponible,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-1"
        };
        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        var existe = await repository.ExisteCodigoAsync("CHECK01");
        var existeExcluyendo = await repository.ExisteCodigoAsync("CHECK01", excludeId: propiedad.Id);
        var disponible = await repository.EstaDisponibleAsync(propiedad.Id);
        var disponibleOtro = await repository.EstaDisponibleAsync(propiedad.Id + 1);

        existe.Should().BeTrue();
        existeExcluyendo.Should().BeFalse();
        disponible.Should().BeTrue();
        disponibleOtro.Should().BeFalse();
    }

    [Fact]
    public async Task GetDetalleByIdAsync_ReturnsPropertyWithRelationships()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "DET001");

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetDetalleByIdAsync(propiedad.Id);

        result.Should().NotBeNull();
        result!.Imagenes.Should().ContainSingle();
        result.PropiedadesMejoras.Should().ContainSingle();
        result.TipoPropiedad.Should().NotBeNull();
        result.TipoVenta.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetalleByIdAsync_ReturnsNullWhenPropertyDoesNotExist()
    {
        using var context = CreateContext();
        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetDetalleByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllWithDetailsAsync_ReturnsOnlyDisponiblesOrdered()
    {
        using var context = CreateContext();
        var disponible = await SeedPropiedadDetalladaAsync(context, "DET200");
        var vendida = await SeedPropiedadDetalladaAsync(context, "DET201", EstadoPropiedad.Vendida);

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetAllWithDetailsAsync();

        result.Should().ContainSingle(p => p.Id == disponible.Id);
        result.Should().NotContain(p => p.Id == vendida.Id);
        result.Should().BeInDescendingOrder(p => p.FechaCreacion);
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ReturnsLoadedProperty()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "DET300");

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByIdWithDetailsAsync(propiedad.Id);

        result.Should().NotBeNull();
        result!.PropiedadesMejoras.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ReturnsNullWhenIdNotFound()
    {
        using var context = CreateContext();
        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByIdWithDetailsAsync(1234);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByFiltrosAsync_ReturnsEmptyWhenNoMatch()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "FIL001");

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByFiltrosAsync(
            tipoPropiedadId: propiedad.TipoPropiedadId,
            precioMin: propiedad.Precio + 50000m,
            precioMax: propiedad.Precio + 60000m,
            habitaciones: propiedad.CantidadHabitaciones + 1,
            banos: propiedad.CantidadBanos + 1);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDisponiblesViewModelAsync_ReturnsMappedList()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "VM001");

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetAllDisponiblesViewModelAsync();

        result.Should().HaveCount(1);
        var vm = result.First();
        vm.Codigo.Should().Be(propiedad.Codigo);
        vm.TipoPropiedad.Should().Contain("Tipo");
        vm.Mejoras.Should().ContainSingle();
        vm.ImagenPrincipal.Should().Contain("VM001");
    }

    [Fact]
    public async Task GetByCodigoViewModelAsync_ReturnsMappedModel()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "VMCOD");

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByCodigoViewModelAsync(propiedad.Codigo);

        result.Should().NotBeNull();
        result!.Codigo.Should().Be(propiedad.Codigo);
        result.TipoPropiedad.Should().Contain("Tipo");
    }

    [Fact]
    public async Task GetByCodigoViewModelAsync_ReturnsNullWhenNotFound()
    {
        using var context = CreateContext();
        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetByCodigoViewModelAsync("DESCONOCIDO");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByFiltrosViewModelAsync_ReturnsMatchingResults()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "VMFIL");

        var repository = new PropiedadRepository(context, Mapper);

        var filtros = new FiltrosPropiedadesViewModel
        {
            TipoPropiedadId = propiedad.TipoPropiedadId,
            PrecioMinimo = propiedad.Precio - 1000m,
            PrecioMaximo = propiedad.Precio + 1000m,
            CantidadHabitaciones = propiedad.CantidadHabitaciones,
            CantidadBanos = propiedad.CantidadBanos
        };

        var result = await repository.GetByFiltrosViewModelAsync(filtros);

        result.Should().ContainSingle(vm => vm.Codigo == propiedad.Codigo);
    }

    [Fact]
    public async Task GetDetalleViewModelByIdAsync_ReturnsProjectedModel()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "VMDET");

        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetDetalleViewModelByIdAsync(propiedad.Id);

        result.Should().NotBeNull();
        result!.Imagenes.Should().ContainSingle();
        result.Mejoras.Should().ContainSingle();
        result.EstaDisponible.Should().BeTrue();
    }

    [Fact]
    public async Task GetDetalleViewModelByIdAsync_ReturnsNullWhenMissing()
    {
        using var context = CreateContext();
        var repository = new PropiedadRepository(context, Mapper);

        var result = await repository.GetDetalleViewModelByIdAsync(9876);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByAgenteIdViewModelAsync_HonorsAvailabilityFlag()
    {
        using var context = CreateContext();
        var disponible = await SeedPropiedadDetalladaAsync(context, "VMAG1", EstadoPropiedad.Disponible, "agente-x");
        var vendida = await SeedPropiedadDetalladaAsync(context, "VMAG2", EstadoPropiedad.Vendida, "agente-x");

        var repository = new PropiedadRepository(context, Mapper);

        var soloDisponibles = await repository.GetByAgenteIdViewModelAsync("agente-x");
        var todas = await repository.GetByAgenteIdViewModelAsync("agente-x", soloDisponibles: false);

        soloDisponibles.Should().ContainSingle(vm => vm.Id == disponible.Id);
        todas.Should().HaveCount(2);
        todas.Should().Contain(vm => vm.Id == vendida.Id);
    }

    [Fact]
    public async Task AddPropiedadMejoraAsync_PersistsRelation()
    {
        using var context = CreateContext();
        var tipo = new TipoPropiedad { Nombre = "Casa", Descripcion = "Residencial" };
        var venta = new TipoVenta { Nombre = "Venta", Descripcion = "Contado" };
        var mejora = new Mejora { Nombre = "Jardin", Descripcion = "Exterior" };
        context.TiposPropiedades.Add(tipo);
        context.TiposVentas.Add(venta);
        context.Mejoras.Add(mejora);
        await context.SaveChangesAsync();

        var propiedad = new Propiedad
        {
            Codigo = "MEJADD",
            Precio = 150000m,
            TamanoEnMetros = 120,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            Descripcion = "Casa",
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = "agente-1"
        };
        context.Propiedades.Add(propiedad);
        await context.SaveChangesAsync();

        var repository = new PropiedadRepository(context, Mapper);

        await repository.AddPropiedadMejoraAsync(new PropiedadMejora
        {
            PropiedadId = propiedad.Id,
            MejoraId = mejora.Id
        });

        context.PropiedadesMejoras.ToList().Should().ContainSingle(pm => pm.PropiedadId == propiedad.Id && pm.MejoraId == mejora.Id);
    }

    [Fact]
    public async Task RemovePropiedadMejorasAsync_RemovesExistingRelations()
    {
        using var context = CreateContext();
        var propiedad = await SeedPropiedadDetalladaAsync(context, "MEJREM");
        var repository = new PropiedadRepository(context, Mapper);

        await repository.RemovePropiedadMejorasAsync(propiedad.Id);

        context.PropiedadesMejoras.ToList().Should().BeEmpty();
    }

    [Fact]
    public async Task GetEstadisticasAsync_ReturnsCounts()
    {
        using var context = CreateContext();
        await SeedPropiedadDetalladaAsync(context, "EST001", EstadoPropiedad.Disponible);
        await SeedPropiedadDetalladaAsync(context, "EST002", EstadoPropiedad.Vendida);

        var repository = new PropiedadRepository(context, Mapper);

        var stats = await repository.GetEstadisticasAsync();

        stats.Disponibles.Should().Be(1);
        stats.Vendidas.Should().Be(1);
    }

    private static async Task<Propiedad> SeedPropiedadDetalladaAsync(ApplicationDbContext context, string codigo, EstadoPropiedad estado = EstadoPropiedad.Disponible, string agenteId = "agente-1")
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
            Precio = 175000m,
            TamanoEnMetros = 140,
            CantidadHabitaciones = 3,
            CantidadBanos = 2,
            Descripcion = $"Propiedad {codigo}",
            Estado = estado,
            FechaCreacion = DateTime.UtcNow,
            TipoPropiedadId = tipo.Id,
            TipoVentaId = venta.Id,
            AgenteId = agenteId,
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
