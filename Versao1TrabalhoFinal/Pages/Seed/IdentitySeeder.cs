using Microsoft.AspNetCore.Identity;

namespace Versao1TrabalhoFinal.Seed
{
    /// <summary>
    /// Classe responsável por criar roles e utilizadores iniciais.
    /// </summary>
    public static class IdentitySeeder
    {
        /// <summary>
        /// Executa o seed de roles e utilizadores.
        /// </summary>
        /// <param name="serviceProvider">Service provider da aplicação.</param>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Mecanico", "Vendedor", "Rececionista", "Cliente" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await CreateUserIfNotExists(userManager, "admin@stand.pt", "Admin123!", "Admin");
            await CreateUserIfNotExists(userManager, "mecanico@stand.pt", "Mecanico123!", "Mecanico");
            await CreateUserIfNotExists(userManager, "vendedor@stand.pt", "Vendedor123!", "Vendedor");
            await CreateUserIfNotExists(userManager, "Rececionista@stand.pt", "Rececionista123!", "Rececionista");
            await CreateUserIfNotExists(userManager, "carlos.silva@email.pt", "Cliente123!", "Cliente");
        }

        /// <summary>
        /// Cria um utilizador caso não exista e atribui-lhe a role indicada.
        /// </summary>
        /// <param name="userManager">User manager do Identity.</param>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="password">Password inicial.</param>
        /// <param name="role">Role a atribuir.</param>
        private static async Task CreateUserIfNotExists(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
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

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Contains(role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
