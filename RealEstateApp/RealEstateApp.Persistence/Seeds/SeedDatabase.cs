using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Identity.Entities;
using RealEstateApp.Persistence.Contexts;
using RealEstateApp.Identity.Seeds;

namespace RealEstateApp.Persistence.Seeds;


public static class SeedDatabase
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
     
        await context.Database.MigrateAsync();

        
        await DefaultRoles.SeedAsync(roleManager);
        await DefaultUsers.SeedAsync(userManager);
        
 
        
        await DefaultTipoPropiedades.SeedAsync(context);
        await DefaultTipoVentas.SeedAsync(context);
        await DefaultMejoras.SeedAsync(context);
        
        await context.SaveChangesAsync();
    }
}
