using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdemProdutos
{
    /// <summary>
    /// Página responsável pela edição de produtos em ordens.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Rececionista")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de edição.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registo associado ao formulário.
        /// </summary>
        [BindProperty]
        public Versao1TrabalhoFinal.Models.OrdemProduto OrdemProduto { get; set; } = new();

        /// <summary>
        /// Lista de ordens disponíveis.
        /// </summary>
        public SelectList OrdensSelect { get; set; } = default!;

        /// <summary>
        /// Lista de produtos disponíveis.
        /// </summary>
        public SelectList ProdutosSelect { get; set; } = default!;

        /// <summary>
        /// Carrega o registo para edição.
        /// </summary>
        /// <param name="id">Identificador do registo.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.OrdemProdutos.FindAsync(id);

            if (item == null)
                return NotFound();

            OrdemProduto = item;
            OrdensSelect = new SelectList(await _context.OrdensReparacao.OrderByDescending(o => o.Id).ToListAsync(), "Id", "Id", OrdemProduto.OrdemId);
            ProdutosSelect = new SelectList(await _context.Produtos.OrderBy(p => p.Nome).ToListAsync(), "Id", "Nome", OrdemProduto.ProdutoId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações do registo.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OrdensSelect = new SelectList(await _context.OrdensReparacao.OrderByDescending(o => o.Id).ToListAsync(), "Id", "Id", OrdemProduto.OrdemId);
                ProdutosSelect = new SelectList(await _context.Produtos.OrderBy(p => p.Nome).ToListAsync(), "Id", "Nome", OrdemProduto.ProdutoId);
                return Page();
            }

            _context.Attach(OrdemProduto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
