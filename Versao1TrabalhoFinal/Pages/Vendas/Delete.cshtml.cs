using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Vendas
{
    /// <summary>
    /// Página de eliminação de vendas.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Venda Venda { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
                return NotFound();

            Venda = venda;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);

            if (venda != null)
            {
                _context.Vendas.Remove(venda);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
