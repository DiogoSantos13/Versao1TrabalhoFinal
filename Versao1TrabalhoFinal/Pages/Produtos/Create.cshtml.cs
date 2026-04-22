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
    /// Página responsável pela criação de produtos para venda.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de criação de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Produto em criação.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Lista de fornecedores disponíveis para o produto.
        /// </summary>
        public SelectList FornecedoresOptions { get; set; } = default!;

        /// <summary>
        /// Carrega a página de criação e os fornecedores disponíveis.
        /// </summary>
        /// <returns>Página de criação.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarFornecedoresAsync();
            return Page();
        }

        /// <summary>
        /// Processa a submissão do formulário de criação do produto.
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
                await CarregarFornecedoresAsync();
                return Page();
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto criado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Carrega a lista de fornecedores para o dropdown.
        /// </summary>
        private async Task CarregarFornecedoresAsync()
        {
            var fornecedores = await _context.Fornecedores
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .Select(f => new { f.Id, f.Nome })
                .ToListAsync();

            FornecedoresOptions = new SelectList(fornecedores, "Id", "Nome", Produto.FornecedorId);
        }
    }
}