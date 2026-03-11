using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Fornecedores
{
    /// <summary>
    /// Página de criação de fornecedores.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fornecedor ligado ao formulário.
        /// </summary>
        [BindProperty]
        public Fornecedor Fornecedor { get; set; } = new();

        public void OnGet()
        {
        }

        /// <summary>
        /// Guarda o fornecedor criado.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Fornecedores.Add(Fornecedor);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
