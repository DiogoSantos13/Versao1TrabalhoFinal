using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.GestaoOrcamentos
{
    /// <summary>
    /// Página responsável por listar os pedidos de orçamento do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de listagem de orçamentos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public IndexModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Lista de pedidos de orçamento do cliente.
        /// </summary>
        public IList<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();

        /// <summary>
        /// Carrega os pedidos de orçamento do cliente autenticado.
        /// </summary>
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                Orcamentos = new List<Orcamento>();
                return;
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            if (cliente == null)
            {
                Orcamentos = new List<Orcamento>();
                return;
            }

            Orcamentos = await _context.Orcamentos
                .AsNoTracking()
                .Include(o => o.Veiculo)
                .Where(o => o.ClienteId == cliente.Id)
                .OrderByDescending(o => o.DataCriacao)
                .ToListAsync();
        }
    }
}
