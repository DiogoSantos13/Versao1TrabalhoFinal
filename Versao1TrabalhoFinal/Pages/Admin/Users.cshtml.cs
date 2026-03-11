using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Versao1TrabalhoFinal.Pages.Admin
{
    /// <summary>
    /// Página responsável pela gestão de utilizadores e alteração das respetivas roles.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Inicializa a página de gestão de utilizadores.
        /// </summary>
        /// <param name="userManager">Gestor de utilizadores.</param>
        /// <param name="roleManager">Gestor de roles.</param>
        public UsersModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Lista de utilizadores com a role atual.
        /// </summary>
        public List<UserRoleViewModel> Users { get; set; } = new();

        /// <summary>
        /// Lista de roles disponíveis no sistema.
        /// </summary>
        public List<string> AvailableRoles { get; set; } = new();

        /// <summary>
        /// Identificador do utilizador a alterar.
        /// </summary>
        [BindProperty]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Role selecionada para o utilizador.
        /// </summary>
        [BindProperty]
        public string SelectedRole { get; set; } = string.Empty;

        /// <summary>
        /// Carrega os utilizadores e as roles disponíveis.
        /// </summary>
        public async Task OnGetAsync()
        {
            AvailableRoles = _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToList();

            foreach (var user in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);

                Users.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    CurrentRole = roles.FirstOrDefault() ?? "Sem Role"
                });
            }
        }

        /// <summary>
        /// Altera a role de um utilizador.
        /// </summary>
        /// <returns>Redireciona para a própria página.</returns>
        public async Task<IActionResult> OnPostChangeRoleAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
                return RedirectToPage();

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrWhiteSpace(SelectedRole))
                await _userManager.AddToRoleAsync(user, SelectedRole);

            return RedirectToPage();
        }

        /// <summary>
        /// Modelo auxiliar para apresentar utilizadores com a respetiva role.
        /// </summary>
        public class UserRoleViewModel
        {
            /// <summary>
            /// Identificador do utilizador.
            /// </summary>
            public string UserId { get; set; } = string.Empty;

            /// <summary>
            /// Email do utilizador.
            /// </summary>
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Role atual do utilizador.
            /// </summary>
            public string CurrentRole { get; set; } = string.Empty;
        }
    }
}
