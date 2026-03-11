using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Fornecedores
{
    /// <summary>
    /// Página de listagem de fornecedores.
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
        /// Lista de fornecedores.
        /// </summary>
        public List<Fornecedor> Fornecedores { get; set; } = new();

        /// <summary>
        /// Carrega os fornecedores da base de dados.
        /// </summary>
        public async Task OnGetAsync()
        {
            Fornecedores = await _context.Fornecedores
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }
    }
}
