using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Fornecedores
{
    /// <summary>
    /// Página de edição de fornecedores.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fornecedor ligado ao formulário.
        /// </summary>
        [BindProperty]
        public Fornecedor Fornecedor { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);

            if (fornecedor == null)
                return NotFound();

            Fornecedor = fornecedor;
            return Page();
        }

        /// <summary>
        /// Guarda as alterações do fornecedor.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Fornecedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
