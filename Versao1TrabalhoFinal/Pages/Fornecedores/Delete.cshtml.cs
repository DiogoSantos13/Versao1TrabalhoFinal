using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Fornecedores
{
    /// <summary>
    /// Página de eliminação de fornecedores.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fornecedor a eliminar.
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
        /// Elimina o fornecedor selecionado.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);

            if (fornecedor != null)
            {
                _context.Fornecedores.Remove(fornecedor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
