using Microsoft.AspNetCore.Identity;

namespace Versao1TrabalhoFinal.Seed
{
    /// <summary>
    /// Classe responsável por criar as roles e os utilizadores base do sistema.
    /// </summary>
    public static class IdentitySeeder
    {
        /// <summary>
        /// Executa o seed inicial do ASP.NET Identity.
        /// </summary>
        /// <param name="serviceProvider">Service provider da aplicação.</param>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles =
            {
                "Admin",
                "Mecanico",
                "Colaborador",
                "Cliente",
                "Rececionista",
                "Vendedor",
                "Funcionario"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Erro ao criar role {role}: {errors}");
                    }
                }
            }

            await CreateUserIfNotExists(userManager, "admin@stand.pt", "Admin123!", new[] { "Admin", "Colaborador" });
            await CreateUserIfNotExists(userManager, "mecanico@stand.pt", "Mecanico123!", new[] { "Mecanico", "Colaborador" });
            await CreateUserIfNotExists(userManager, "colaborador@stand.pt", "Colaborador123!", new[] { "Colaborador" });
            await CreateUserIfNotExists(userManager, "rececionista@stand.pt", "Rececionista123!", new[] { "Rececionista", "Colaborador" });
            await CreateUserIfNotExists(userManager, "vendedor@stand.pt", "Vendedor123!", new[] { "Vendedor", "Colaborador" });
            await CreateUserIfNotExists(userManager, "funcionario@stand.pt", "Funcionario123!", new[] { "Funcionario", "Colaborador" });
            await CreateUserIfNotExists(userManager, "cliente@stand.pt", "Cliente123!", new[] { "Cliente" });
        }

        /// <summary>
        /// Cria um utilizador caso ainda não exista e garante a atribuição das roles indicadas.
        /// </summary>
        /// <param name="userManager">Gestor de utilizadores.</param>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="password">Password inicial.</param>
        /// <param name="roles">Roles a atribuir ao utilizador.</param>
        private static async Task CreateUserIfNotExists(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            IEnumerable<string> roles)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao criar utilizador {email}: {errors}");
                }
            }

            foreach (var role in roles)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, role);

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Erro ao atribuir role {role} a {email}: {errors}");
                    }
                }
            }
        }
    }
}