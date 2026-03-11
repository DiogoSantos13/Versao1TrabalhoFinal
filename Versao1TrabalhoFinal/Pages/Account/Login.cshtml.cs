using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Versao1TrabalhoFinal.Pages.Account
{
    /// <summary>
    /// Página responsável pela autenticação de utilizadores.
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de login.
        /// </summary>
        /// <param name="signInManager">Gestor de autenticação.</param>
        /// <param name="userManager">Gestor de utilizadores.</param>
        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Dados introduzidos no formulário.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Modelo do formulário de autenticação.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Email do utilizador.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Palavra-passe do utilizador.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// Indica se a sessão deve ser memorizada.
            /// </summary>
            [Display(Name = "Lembrar sessão")]
            public bool RememberMe { get; set; }
        }

        /// <summary>
        /// Carrega a página de login.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Processa o pedido de autenticação do utilizador.
        /// </summary>
        /// <returns>Redireciona para a área correta conforme a role.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Login inválido.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToPage("/Dashboard/Index");

            if (await _userManager.IsInRoleAsync(user, "Rececionista"))
                return RedirectToPage("/OrdensReparacao/Index");

            if (await _userManager.IsInRoleAsync(user, "Mecanico"))
                return RedirectToPage("/Servicos/Index");

            if (await _userManager.IsInRoleAsync(user, "Vendedor"))
                return RedirectToPage("/Vendas/Index");

            if (await _userManager.IsInRoleAsync(user, "Cliente"))
                return RedirectToPage("/ClienteArea/Veiculos");

            return RedirectToPage("/Index");
        }
    }
}
