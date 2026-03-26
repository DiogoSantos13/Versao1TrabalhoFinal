using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.ClienteArea
{
    /// <summary>
    /// Página que lista os pedidos e orçamentos do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class OrcamentosModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        public OrcamentosModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de orçamentos do cliente autenticado.
        /// </summary>
        public List<Versao1TrabalhoFinal.Models.Orcamento> Orcamentos { get; set; } = new();

        /// <summary>
        /// Carrega os orçamentos do cliente atual.
        /// </summary>
        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;

            Orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .Where(o => o.Cliente != null && o.Cliente.Email == email)
                //.OrderByDescending(o => o.DataPedido)
                .ToListAsync();
        }
    }
}
