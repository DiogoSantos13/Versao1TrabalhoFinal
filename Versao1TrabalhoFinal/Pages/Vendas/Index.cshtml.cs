using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Vendas
{
    /// <summary>
    /// Página de listagem de vendas.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de vendas.
        /// </summary>
        public List<Venda> Vendas { get; set; } = new();

        public async Task OnGetAsync()
        {
            Vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }
    }
}
