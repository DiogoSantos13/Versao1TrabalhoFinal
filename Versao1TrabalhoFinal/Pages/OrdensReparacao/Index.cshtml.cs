using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdensReparacao
{
    /// <summary>
    /// Página de listagem de ordens de reparação.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de ordens de reparação.
        /// </summary>
        public List<OrdemReparacao> Ordens { get; set; } = new();

        /// <summary>
        /// Carrega as ordens existentes com cliente e veículo.
        /// </summary>
        public async Task OnGetAsync()
        {
            Ordens = await _context.OrdensReparacao
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .OrderByDescending(o => o.DataEntrada)
                .ToListAsync();
        }
    }
}
