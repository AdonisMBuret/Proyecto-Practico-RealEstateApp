using Microsoft.AspNetCore.Identity;
using RealEstateApp.Identity.Entities;

namespace RealEstateApp.Identity.Seeds;

public static class DefaultUsers
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserAsync(userManager, new ApplicationUser
        {
            Nombre = "Admin",
            Apellido = "Sistema",
            UserName = "admin",
            Email = "admin@realestate.com",
            PhoneNumber = "8091234567",
            EsActivo = true,
            EmailConfirmed = true,
            Cedula = "00100000000"
        }, "Admin123!", DefaultRoles.ADMIN);

        await CreateUserAsync(userManager, new ApplicationUser
        {
            Nombre = "Cliente",
            Apellido = "Prueba",
            UserName = "cliente",
            Email = "cliente@realestate.com",
            PhoneNumber = "8091234568",
            EsActivo = true,
            EmailConfirmed = true
        }, "Cliente123!", DefaultRoles.CLIENT);

        await CreateUserAsync(userManager, new ApplicationUser
        {
            Nombre = "Agente",
            Apellido = "Prueba",
            UserName = "agente",
            Email = "agente@realestate.com",
            PhoneNumber = "8091234569",
            EsActivo = true,
            EmailConfirmed = true
        }, "Agente123!", DefaultRoles.AGENT);

        await CreateUserAsync(userManager, new ApplicationUser
        {
            Nombre = "Desarrollador",
            Apellido = "Sistema",
            UserName = "developer",
            Email = "developer@realestate.com",
            PhoneNumber = "8091234570",
            EsActivo = true,
            EmailConfirmed = true,
            Cedula = "00200000000"
        }, "Developer123!", DefaultRoles.DEVELOPER);
    }

    private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role)
    {
        if (await userManager.FindByNameAsync(user.UserName!) == null)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}