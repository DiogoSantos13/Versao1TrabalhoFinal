using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VendaItens
{
    /// <summary>
    /// Página responsável pela edição de um item de venda.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de edição de item de venda.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Item de venda associado ao formulário.
        /// </summary>
        [BindProperty]
        public VendaItem VendaItem { get; set; } = new();

        /// <summary>
        /// Lista de vendas disponíveis.
        /// </summary>
        public SelectList VendasSelect { get; set; } = default!;

        /// <summary>
        /// Lista de produtos disponíveis.
        /// </summary>
        public SelectList ProdutosSelect { get; set; } = default!;

        /// <summary>
        /// Carrega o item de venda para edição.
        /// </summary>
        /// <param name="id">Identificador do item.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VendaItens.FindAsync(id);

            if (item == null)
                return NotFound();

            VendaItem = item;
            VendasSelect = new SelectList(await _context.Vendas.OrderByDescending(v => v.Id).ToListAsync(), "Id", "Id", VendaItem.VendaId);
            ProdutosSelect = new SelectList(await _context.Produtos.OrderBy(p => p.Nome).ToListAsync(), "Id", "Nome", VendaItem.ProdutoId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações do item de venda.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                VendasSelect = new SelectList(await _context.Vendas.OrderByDescending(v => v.Id).ToListAsync(), "Id", "Id", VendaItem.VendaId);
                ProdutosSelect = new SelectList(await _context.Produtos.OrderBy(p => p.Nome).ToListAsync(), "Id", "Nome", VendaItem.ProdutoId);
                return Page();
            }

            _context.Attach(VendaItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
