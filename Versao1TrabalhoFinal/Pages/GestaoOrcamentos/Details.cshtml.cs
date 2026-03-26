using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.GestaoOrcamentos
{
    /// <summary>
    /// Página responsável por apresentar o detalhe de um pedido de orçamento do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class DetailsModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de detalhe de pedidos de orçamento.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores do ASP.NET Core Identity.</param>
        public DetailsModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Pedido de orçamento a apresentar na página.
        /// </summary>
        public Orcamento Orcamento { get; set; } = default!;

        /// <summary>
        /// Carrega o detalhe do pedido de orçamento, validando se pertence ao cliente autenticado.
        /// </summary>
        /// <param name="id">Identificador do pedido de orçamento.</param>
        /// <returns>Resultado da execução da página.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Não foi encontrado um cliente associado à conta autenticada.";
                return RedirectToPage("./Index");
            }

            var orcamento = await _context.Orcamentos
                .AsNoTracking()
                .Include(o => o.Veiculo)
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id && o.ClienteId == cliente.Id);

            if (orcamento == null)
            {
                return NotFound();
            }

            Orcamento = orcamento;
            return Page();
        }
    }
}
