using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Versao1TrabalhoFinal.Pages.Account
{
    /// <summary>
    /// Página responsável pelo logout do utilizador autenticado.
    /// </summary>
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Construtor da página de logout.
        /// </summary>
        public LogoutModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// Termina a sessão e redireciona para a página inicial.
        /// </summary>
        /// <returns>Redirecionamento para a homepage.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}

