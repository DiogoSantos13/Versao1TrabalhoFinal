using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages
{
    /// <summary>
    /// Página inicial da aplicação.
    /// Apresenta as últimas viaturas, serviços em destaque e produtos em destaque.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página inicial.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista das viaturas mais recentes para apresentar na home page.
        /// </summary>
        public List<VeiculoStand> VeiculosDestaque { get; set; } = new();

        /// <summary>
        /// Lista dos serviços em destaque para apresentar na home page.
        /// </summary>
        public List<Servico> ServicosDestaque { get; set; } = new();

        /// <summary>
        /// Lista dos produtos em destaque para apresentar na home page.
        /// </summary>
        public List<Produto> ProdutosDestaque { get; set; } = new();

        /// <summary>
        /// Handler GET responsável por carregar os dados da página inicial.
        /// </summary>
        public async Task OnGetAsync()
        {
            VeiculosDestaque = await _context.VeiculosStand
                .AsNoTracking()
                .OrderByDescending(v => v.DataEntrada)
                .Take(6)
                .ToListAsync();

            ServicosDestaque = await _context.Servicos
                .AsNoTracking()
                .Where(s => s.Ativo)
                .OrderBy(s => s.Nome)
                .Take(6)
                .ToListAsync();

            ProdutosDestaque = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Stock > 0)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();
        }
    }
}