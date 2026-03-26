using Microsoft.AspNetCore.Identity;

namespace Versao1TrabalhoFinal.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Mecanico", "Colaborador", "Cliente" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await CreateUserIfNotExists(userManager, "admin@stand.pt", "Admin123!", "Admin");
            await CreateUserIfNotExists(userManager, "mecanico@stand.pt", "Mecanico123!", "Mecanico");
            await CreateUserIfNotExists(userManager, "colaborador@stand.pt", "Colaborador123!", "Colaborador");
            await CreateUserIfNotExists(userManager, "cliente@stand.pt", "Cliente123!", "Cliente");
        }

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

                if (!result.Succeeded)
                {
                    var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao criar utilizador {email}: {errors}");
                }
            }

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
