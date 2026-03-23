using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Identity.Seeds;

public static class DefaultRoles
{
    public const string ADMIN = "Administrador";
    public const string AGENT = "Agente";
    public const string CLIENT = "Cliente";
    public const string DEVELOPER = "Desarrollador";

    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        await CreateRoleAsync(roleManager, ADMIN);
        await CreateRoleAsync(roleManager, AGENT);
        await CreateRoleAsync(roleManager, CLIENT);
        await CreateRoleAsync(roleManager, DEVELOPER);
    }

    private static async Task CreateRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}