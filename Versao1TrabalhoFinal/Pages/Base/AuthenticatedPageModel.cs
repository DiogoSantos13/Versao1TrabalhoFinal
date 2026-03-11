using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Versao1TrabalhoFinal.Pages.Base
{
    /// <summary>
    /// Classe base para páginas autenticadas.
    /// </summary>
    public abstract class AuthenticatedPageModel : PageModel
    {
        /// <summary>
        /// Email do utilizador autenticado.
        /// </summary>
        public string? UserEmail => User.Identity?.Name;

        /// <summary>
        /// Indica se o utilizador atual é cliente.
        /// </summary>
        public bool IsCliente => User.IsInRole("Cliente");

        /// <summary>
        /// Indica se o utilizador atual é administrador.
        /// </summary>
        public bool IsAdmin => User.IsInRole("Admin");
    }
}
