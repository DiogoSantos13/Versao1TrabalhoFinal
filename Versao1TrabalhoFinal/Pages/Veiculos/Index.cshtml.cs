using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    /// <summary>
    /// Página responsável pela listagem dos veículos do utilizador autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente,Admin,Mecanico,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de listagem de veículos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public IndexModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Lista de veículos a apresentar na página.
        /// </summary>
        public List<Veiculo> Veiculos { get; set; } = new();

        /// <summary>
        /// Carrega os veículos associados ao cliente autenticado.
        /// </summary>
        /// <returns>A página com os veículos do utilizador ou um redirecionamento apropriado.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Veiculos = await _context.Veiculos
                .AsNoTracking()
                .Where(v => v.ClienteId == cliente.Id)
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .ToListAsync();

            return Page();
        }
    }
}
