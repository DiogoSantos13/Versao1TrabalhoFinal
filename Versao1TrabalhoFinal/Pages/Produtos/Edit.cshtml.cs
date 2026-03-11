using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Produtos
{
    /// <summary>
    /// Página responsável pela edição de produtos.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Produto a editar.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Lista de fornecedores.
        /// </summary>
        public SelectList FornecedoresSelect { get; set; } = default!;

        /// <summary>
        /// Carrega o produto para edição.
        /// </summary>
        /// <param name="id">Id do produto.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return NotFound();

            Produto = produto;

            FornecedoresSelect = new SelectList(
                await _context.Fornecedores.OrderBy(f => f.Nome).ToListAsync(),
                "Id",
                "Nome",
                Produto.FornecedorId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações do produto.
        /// </summary>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                FornecedoresSelect = new SelectList(
                    await _context.Fornecedores.OrderBy(f => f.Nome).ToListAsync(),
                    "Id",
                    "Nome",
                    Produto.FornecedorId);

                return Page();
            }

            _context.Attach(Produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
