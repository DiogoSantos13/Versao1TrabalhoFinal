using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VendaItens
{
    /// <summary>
    /// Página responsável pela listagem dos itens de venda.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de listagem de itens de venda.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de itens de venda carregados da base de dados.
        /// </summary>
        public List<VendaItem> Itens { get; set; } = new();

        /// <summary>
        /// Carrega os itens de venda existentes.
        /// </summary>
        public async Task OnGetAsync()
        {
            Itens = await _context.Set<VendaItem>()
                .Include(v => v.Venda)
                .Include(v => v.Produto)
                .OrderByDescending(v => v.Id)
                .ToListAsync();
        }
    }
}

