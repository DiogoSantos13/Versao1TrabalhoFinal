using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Versao1TrabalhoFinal.Pages.Account
{
    /// <summary>
    /// Página responsável pela autenticação de utilizadores na aplicação.
    /// </summary>
    public class LoginModel : PageModel
    {
        /// <summary>
        /// Serviço responsável por autenticar utilizadores.
        /// </summary>
        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Serviço responsável pela gestão dos utilizadores do Identity.
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de login.
        /// </summary>
        /// <param name="signInManager">Serviço de autenticação.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores.</param>
        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Dados introduzidos no formulário de login.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// URL de retorno usada para redirecionar o utilizador após autenticação.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Modelo com os campos do formulário de autenticação.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Email do utilizador.
            /// </summary>
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Introduza um email válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Palavra-passe do utilizador.
            /// </summary>
            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// Indica se a sessão deve ser mantida iniciada.
            /// </summary>
            [Display(Name = "Lembrar sessão")]
            public bool RememberMe { get; set; }
        }

        /// <summary>
        /// Carrega a página de login.
        /// Se o utilizador já estiver autenticado, redireciona para a página inicial.
        /// </summary>
        /// <param name="returnUrl">URL de retorno opcional.</param>
        /// <returns>Resultado da página ou redirecionamento.</returns>
        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            ReturnUrl = returnUrl;
            return Page();
        }

        /// <summary>
        /// Processa a tentativa de autenticação do utilizador.
        /// </summary>
        /// <param name="returnUrl">URL de retorno opcional.</param>
        /// <returns>Redirecionamento após autenticação ou retorno à página com erro.</returns>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Valida os campos do formulário.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Procura o utilizador pelo email introduzido.
            var user = await _userManager.FindByEmailAsync(Input.Email);

            // Se o utilizador não existir, devolve erro.
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Login inválido.");
                return Page();
            }

            // Tenta autenticar o utilizador com username e palavra-passe.
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            // Se a autenticação falhar, devolve erro.
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Login inválido.");
                return Page();
            }

            // Se existir uma URL local de retorno, redireciona para ela.
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            // Redireciona conforme o papel do utilizador autenticado.
            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToPage("/Dashboard/Index");

            if (await _userManager.IsInRoleAsync(user, "Rececionista"))
                return RedirectToPage("/OrdensReparacao/Index");

            if (await _userManager.IsInRoleAsync(user, "Mecanico"))
                return RedirectToPage("/Servicos/Index");

            if (await _userManager.IsInRoleAsync(user, "Vendedor"))
                return RedirectToPage("/Vendas/Index");

            // Cliente autenticado é redirecionado automaticamente para VeiculosStand.
            if (await _userManager.IsInRoleAsync(user, "Cliente"))
                return RedirectToPage("/VeiculosStand/Index");

            // Caso não tenha um papel esperado, redireciona para a página inicial.
            return RedirectToPage("/Index");
        }
    }
}