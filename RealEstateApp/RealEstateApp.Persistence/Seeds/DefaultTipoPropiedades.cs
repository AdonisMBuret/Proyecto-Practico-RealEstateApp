using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Seeds;

public static class DefaultTipoPropiedades
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.TiposPropiedades.AnyAsync())
        {
            var tiposPropiedades = new List<TipoPropiedad>
            {
               
                new TipoPropiedad { Nombre = "Casa", Descripcion = "Vivienda unifamiliar independiente" },
                new TipoPropiedad { Nombre = "Apartamento", Descripcion = "Vivienda en edificio multifamiliar" },
                new TipoPropiedad { Nombre = "Local Comercial", Descripcion = "Espacio para actividades comerciales" },
                new TipoPropiedad { Nombre = "Terreno", Descripcion = "Lote de tierra sin construcción" }
            };

            await context.TiposPropiedades.AddRangeAsync(tiposPropiedades);
            await context.SaveChangesAsync();
        }
    }
}
