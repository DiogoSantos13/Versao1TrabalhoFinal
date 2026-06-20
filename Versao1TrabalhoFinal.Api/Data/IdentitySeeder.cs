using Microsoft.AspNetCore.Identity;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Responsável pela criação inicial de roles e do utilizador administrador.
    /// </summary>
    public static class IdentitySeeder
    {
        /// <summary>
        /// Cria as roles base e o utilizador administrador.
        /// </summary>
        /// <param name="services">Provider de serviços da aplicação.</param>
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Funcionario", "Cliente" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@standoficina.pt";
            var adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                var currentRoles = await userManager.GetRolesAsync(adminUser);

                if (!currentRoles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}