using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdemProdutos
{
    /// <summary>
    /// Página responsável pela listagem dos produtos associados às ordens de reparação.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Rececionista")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de listagem de produtos em ordens.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de produtos associados a ordens.
        /// Uso de nome totalmente qualificado para evitar conflito com namespace `OrdemProduto`.
        /// </summary>
        public List<global::Versao1TrabalhoFinal.Models.OrdemProduto> OrdemProdutos { get; set; } = new();

        /// <summary>
        /// Carrega os registos existentes.
        /// Correção: usa DbContext.Set&lt;T&gt;() em vez de uma propriedade DbSet que pode não existir no contexto.
        /// </summary>
        public async Task OnGetAsync()
        {
            OrdemProdutos = await _context
                .Set<global::Versao1TrabalhoFinal.Models.OrdemProduto>()
                .Include(o => o.OrdemReparacao)
                .Include(o => o.Produto)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
        }
    }
}
