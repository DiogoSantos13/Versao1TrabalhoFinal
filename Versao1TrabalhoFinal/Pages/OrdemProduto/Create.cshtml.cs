using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdemProdutos
{
    /// <summary>
    /// Página responsável pela criação de produtos em ordens de reparação.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Rececionista")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de criação.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CreateModel(StandDbContext context)
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
        /// Carrega os dados necessários para o formulário.
        /// </summary>
        public void OnGet()
        {
            OrdensSelect = new SelectList(_context.OrdensReparacao.OrderByDescending(o => o.Id).ToList(), "Id", "Id");
            ProdutosSelect = new SelectList(_context.Produtos.OrderBy(p => p.Nome).ToList(), "Id", "Nome");
        }

        /// <summary>
        /// Processa a criação do registo.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OrdensSelect = new SelectList(_context.OrdensReparacao.OrderByDescending(o => o.Id).ToList(), "Id", "Id");
                ProdutosSelect = new SelectList(_context.Produtos.OrderBy(p => p.Nome).ToList(), "Id", "Nome");
                return Page();
            }

            _context.OrdemProdutos.Add(OrdemProduto);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
