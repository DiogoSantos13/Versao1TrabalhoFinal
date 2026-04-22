using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Produtos
{
    /// <summary>
    /// Página responsável pela eliminação de produtos.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de eliminação de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Produto a eliminar.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Carrega o produto antes da confirmação da eliminação.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var produto = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            Produto = produto;
            return Page();
        }

        /// <summary>
        /// Elimina o produto selecionado.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto eliminado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}