using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VendaItens
{
    /// <summary>
    /// Página responsável pela eliminação de um item de venda.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de eliminação de item de venda.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Item de venda a eliminar.
        /// </summary>
        [BindProperty]
        public VendaItem VendaItem { get; set; } = new();

        /// <summary>
        /// Carrega o item antes da confirmação.
        /// </summary>
        /// <param name="id">Identificador do item.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VendaItens
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (item == null)
                return NotFound();

            VendaItem = item;
            return Page();
        }

        /// <summary>
        /// Elimina o item de venda selecionado.
        /// </summary>
        /// <param name="id">Identificador do item.</param>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var item = await _context.VendaItens.FindAsync(id);

            if (item != null)
            {
                _context.VendaItens.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
