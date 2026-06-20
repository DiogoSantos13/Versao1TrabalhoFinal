using Microsoft.AspNetCore.Identity;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Classe responsável por inicializar roles e dados base.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Cria as roles base da aplicação.
        /// </summary>
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Funcionario", "Cliente" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}