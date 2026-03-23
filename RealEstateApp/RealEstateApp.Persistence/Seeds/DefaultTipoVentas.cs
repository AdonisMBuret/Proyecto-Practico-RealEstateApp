using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Seeds;

public static class DefaultTipoVentas
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.TiposVentas.AnyAsync())
        {
            var tiposVentas = new List<TipoVenta>
            {
                new TipoVenta { Nombre = "Venta", Descripcion = "Venta definitiva de la propiedad" },
                new TipoVenta { Nombre = "Alquiler", Descripcion = "Alquiler mensual de la propiedad" }
            };

            await context.TiposVentas.AddRangeAsync(tiposVentas);
            await context.SaveChangesAsync();
        }
    }
}
