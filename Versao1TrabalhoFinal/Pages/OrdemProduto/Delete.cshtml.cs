using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdemProdutos
{
    /// <summary>
    /// Página responsável pela eliminação de produtos em ordens.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Rececionista")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de eliminação.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registo a eliminar.
        /// </summary>
        [BindProperty]
        public Versao1TrabalhoFinal.Models.OrdemProduto OrdemProduto { get; set; } = new();


        /// <summary>
        /// Carrega o registo antes da confirmação.
        /// </summary>
        /// <param name="id">Identificador do registo.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Qualificar o tipo para evitar conflito com um namespace chamado 'OrdemProduto'
            var item = await _context
                .Set<Versao1TrabalhoFinal.Models.OrdemProduto>()
                .Include(o => o.Produto)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (item == null)
                return NotFound();

            OrdemProduto = item;
            return Page();
        }

        /// <summary>
        /// Elimina o registo selecionado.
        /// </summary>
        /// <param name="id">Identificador do registo.</param>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var item = await _context.Set<global::Versao1TrabalhoFinal.Models.OrdemProduto>().FindAsync(id);

            if (item != null)
            {
                _context.Set<global::Versao1TrabalhoFinal.Models.OrdemProduto>().Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
