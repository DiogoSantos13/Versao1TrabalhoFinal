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
        /// Inicializa uma nova instância da página de edição de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Produto que está a ser editado.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Lista de fornecedores disponíveis para o dropdown.
        /// </summary>
        public SelectList FornecedoresSelect { get; set; } = default!;

        /// <summary>
        /// Carrega o produto e os fornecedores para edição.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <returns>Página de edição ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var produto = await _context.Produtos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            Produto = produto;
            await CarregarFornecedoresAsync(Produto.FornecedorId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações efetuadas ao produto.
        /// </summary>
        /// <returns>Redireciona para a listagem em caso de sucesso.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            var fornecedorValido = await _context.Fornecedores
                .AsNoTracking()
                .AnyAsync(f => f.Id == Produto.FornecedorId);

            if (!fornecedorValido)
            {
                ModelState.AddModelError("Produto.FornecedorId", "Selecione um fornecedor válido.");
            }

            if (!ModelState.IsValid)
            {
                await CarregarFornecedoresAsync(Produto.FornecedorId);
                return Page();
            }

            _context.Attach(Produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var produtoExiste = await _context.Produtos
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == Produto.Id);

                if (!produtoExiste)
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = "Produto atualizado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Carrega a lista de fornecedores e seleciona o atual, se existir.
        /// </summary>
        /// <param name="fornecedorSelecionadoId">Fornecedor atualmente selecionado.</param>
        private async Task CarregarFornecedoresAsync(int? fornecedorSelecionadoId = null)
        {
            var fornecedores = await _context.Fornecedores
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .ToListAsync();

            FornecedoresSelect = new SelectList(
                fornecedores,
                "Id",
                "Nome",
                fornecedorSelecionadoId);
        }
    }
}