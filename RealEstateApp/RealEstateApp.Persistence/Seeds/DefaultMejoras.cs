using Microsoft.EntityFrameworkCore;
using RealEstateApp.Domain.Entities;
using RealEstateApp.Persistence.Contexts;

namespace RealEstateApp.Persistence.Seeds;

public static class DefaultMejoras
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Mejoras.AnyAsync())
        {
            var mejoras = new List<Mejora>
            {
                new Mejora { Nombre = "Piscina", Descripcion = "Piscina privada o común" },
                new Mejora { Nombre = "Parqueo", Descripcion = "Espacio de estacionamiento" },
                new Mejora { Nombre = "Jardín", Descripcion = "Área verde privada o común" },
                new Mejora { Nombre = "Terraza", Descripcion = "Espacio exterior techado o descubierto" },
                new Mejora { Nombre = "Aire Acondicionado", Descripcion = "Sistema de climatización central o individual" },
                new Mejora { Nombre = "Seguridad 24/7", Descripcion = "Vigilancia y control de acceso permanente" }
            };

            await context.Mejoras.AddRangeAsync(mejoras);
            await context.SaveChangesAsync();
        }
    }
}
