using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VendaItens
{
    /// <summary>
    /// Página responsável pela criação de um item de venda.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de criação de item de venda.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CreateModel(StandDbContext context)
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
        /// Carrega os dados necessários ao formulário.
        /// </summary>
        public void OnGet()
        {
            VendasSelect = new SelectList(_context.Vendas.OrderByDescending(v => v.Id).ToList(), "Id", "Id");
            ProdutosSelect = new SelectList(_context.Produtos.OrderBy(p => p.Nome).ToList(), "Id", "Nome");
        }

        /// <summary>
        /// Processa a criação do item de venda.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                VendasSelect = new SelectList(_context.Vendas.OrderByDescending(v => v.Id).ToList(), "Id", "Id");
                ProdutosSelect = new SelectList(_context.Produtos.OrderBy(p => p.Nome).ToList(), "Id", "Nome");
                return Page();
            }

            _context.VendaItens.Add(VendaItem);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
