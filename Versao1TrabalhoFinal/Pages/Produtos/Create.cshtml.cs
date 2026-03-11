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
    /// Página responsável pela criação de produtos.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Produto ligado ao formulário.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Lista de fornecedores para dropdown.
        /// </summary>
        public SelectList FornecedoresSelect { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do formulário.
        /// </summary>
        public async Task OnGetAsync()
        {
            FornecedoresSelect = new SelectList(
                await _context.Fornecedores.OrderBy(f => f.Nome).ToListAsync(),
                "Id",
                "Nome");
        }

        /// <summary>
        /// Guarda o produto criado.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                FornecedoresSelect = new SelectList(
                    await _context.Fornecedores.OrderBy(f => f.Nome).ToListAsync(),
                    "Id",
                    "Nome");
                return Page();
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
